#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import "UnityAppController.h"

static const char* UNITY_MESSAGE_CALLBACK_MANAGER = "IDFAPlatformIOSImpl";
static const char* UNITY_MESSAGE_CALLBACK_METHOD = "OnTrackingAuthorizationComplete";

static const char* UNITY_MESSAGE_STATUS_UNSUPPORTED_VERSION = "UnsupportedVersion";

void IDFATrackingSendMessage(const char* status)
{
    UnitySendMessage(UNITY_MESSAGE_CALLBACK_MANAGER, UNITY_MESSAGE_CALLBACK_METHOD, status);
}

void IDFATrackingRequestAuthorization()
{
  if(@available(iOS 14, *))
  {
    [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
        NSString* status_str = [NSString stringWithFormat:@"%lu", (unsigned long)status];
        const char* status_utf8 = [status_str UTF8String];
        UnitySendMessage(UNITY_MESSAGE_CALLBACK_MANAGER, UNITY_MESSAGE_CALLBACK_METHOD, status_utf8);
    }];
  }
  else
  {
      IDFATrackingSendMessage(UNITY_MESSAGE_STATUS_UNSUPPORTED_VERSION);
  }
}

const char* MakeRetvalString(const char* src)
{
    size_t str_len = strlen(src);
    size_t byte_size = str_len+1; //+1 for terminating 0 byte
    char* retval = malloc(byte_size); //free'd by il2cpp interop layer
    retval[str_len] = 0;
    strncpy(retval, src, str_len);
    return retval;
}

const char* IDFATRackingGetCurrentStatus()
{
    if(@available(iOS 14, *))
    {
        NSUInteger status = [ATTrackingManager trackingAuthorizationStatus];
        NSString* status_str = [NSString stringWithFormat:@"%lu", (unsigned long)status];
        const char* status_utf8 = [status_str UTF8String];
        return MakeRetvalString(status_utf8);
    }
    else
    {
        return MakeRetvalString(UNITY_MESSAGE_STATUS_UNSUPPORTED_VERSION);
    }
}
