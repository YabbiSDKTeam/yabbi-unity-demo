#if defined(__has_include) && __has_include("UnityInterface.h")
#import "UnityInterface.h"
#endif
#import "InterstitialDelegate.h"



@implementation InterstitialDelegate

-(void) onInterstitialLoaded {
    if(self.onInterstitialLoadedCallback) {
        self.onInterstitialLoadedCallback();
    }
}

- (void)onInterstitialLoadFailed:(UnfiledAdException * _Nonnull)error {
    if(self.onInterstitialLoadFailedCallback) {
        self.onInterstitialLoadFailedCallback([error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onInterstitialShown {
    if(self.onInterstitialShownCallback) {
        self.onInterstitialShownCallback();
    }
}

- (void)onInterstitialShowFailed:(UnfiledAdException * _Nonnull)error {
    if(self.onInterstitialShowFailedCallback) {
        self.onInterstitialShowFailedCallback([error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onInterstitialClosed {
    if(self.onInterstitialClosedCallback) {
        self.onInterstitialClosedCallback();
    }
}

@end

