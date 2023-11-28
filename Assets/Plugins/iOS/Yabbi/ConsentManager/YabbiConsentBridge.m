#if defined(__has_include) && __has_include("UnityAppController.h")
#import "UnityAppController.h"
#endif
#import <Foundation/Foundation.h>
#import <YabbiConsentManager/YabbiConsentManager-Swift.h>
#import "YabbiConsentDelegate.h"

UIViewController *YabbiConsentRootViewController(void) {
#if defined(__has_include) && __has_include("UnityAppController.h")
    return ((UnityAppController *)[UIApplication sharedApplication].delegate).rootViewController;
#else
    return NULL;
#endif
}

void YabbiLoadConsent(void) {
    [ConsentManager loadManager];
}

void YabbiShowConsent(void) {
    [ConsentManager showConsentWindow:YabbiConsentRootViewController()];
}

BOOL YabbiHasConsent(void) {
    return [ConsentManager hasConsent];
}

void YabbiConsentEnableDebug(BOOL isDebug){
    [ConsentManager enableLog:isDebug];
}

void YabbiRegisterCustomConsentVendor(const char *appName, const char *policyUrl, const char *bundle, BOOL isGdpr){
    [ConsentManager registerCustomVendor:^(ConsentBuilder* builder) {
        (void)[builder appendName:[NSString stringWithUTF8String:appName]];
        (void)[builder appendPolicyURL:[NSString stringWithUTF8String:policyUrl]];
        (void)[builder appendBundle:[NSString stringWithUTF8String:bundle]];
        (void)[builder appendGDPR:isGdpr];
    }];
}

static YabbiConsentDelegate *ConsentDelegateInstance;
void YabbiSetConsentDelegate(
                                  ConsentCallbacks onConsentManagerLoaded,
                                  ConsentFailCallbacks onConsentManagerLoadFailed,
                                  ConsentCallbacks onConsentWindowShown,
                                  ConsentFailCallbacks onConsentManagerShownFailed,
                                  ConsenClosedCallbacks onConsentWindowClosed
                                  ) {
    
    ConsentDelegateInstance = [YabbiConsentDelegate new];
    
    ConsentDelegateInstance.onConsentManagerLoadedCallback = onConsentManagerLoaded;
    ConsentDelegateInstance.onConsentManagerLoadFailedCallback = onConsentManagerLoadFailed;
    ConsentDelegateInstance.onConsentWindowShownCallback = onConsentWindowShown;
    ConsentDelegateInstance.onConsentManagerShownFailedCallback = onConsentManagerShownFailed;
    ConsentDelegateInstance.onConsentWindowClosedCallback = onConsentWindowClosed;
    
    
    [ConsentManager setDelegate:ConsentDelegateInstance];
}
