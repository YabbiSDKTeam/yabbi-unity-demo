#if defined(__has_include) && __has_include("UnityInterface.h")
#import "UnityInterface.h"
#endif
#import "YabbiConsentDelegate.h"

@implementation YabbiConsentDelegate

- (void)onConsentManagerLoaded {
    if(self.onConsentManagerLoadedCallback) {
        self.onConsentManagerLoadedCallback();
    }
}

- (void)onConsentManagerLoadFailed:(NSString *)error {
    if(self.onConsentManagerLoadFailedCallback) {
        self.onConsentManagerLoadFailedCallback([error UTF8String]);
    }
}

- (void)onConsentWindowShown {
    if(self.onConsentWindowShownCallback) {
        self.onConsentWindowShownCallback();
    }
}

- (void)onConsentManagerShownFailed:(NSString *)error {
    if(self.onConsentManagerShownFailedCallback) {
        self.onConsentManagerShownFailedCallback([error UTF8String]);
    }
}

- (void)onConsentWindowClosed:(BOOL)hasConsent {
    if(self.onConsentWindowClosedCallback) {
        self.onConsentWindowClosedCallback(hasConsent);
    }
}

@end
