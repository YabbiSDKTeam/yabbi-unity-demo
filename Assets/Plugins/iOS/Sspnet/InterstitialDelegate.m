#if defined(__has_include) && __has_include("UnityInterface.h")
#import "UnityInterface.h"
#endif
#import "InterstitialDelegate.h"



@implementation InterstitialDelegate

- (void)onInterstitialClosed:(UnfiledAdPayload * _Nonnull)adPayload { 
    if(self.onInterstitialClosedCallback) {
        self.onInterstitialClosedCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onInterstitialLoadFailed:(UnfiledAdPayload * _Nonnull)adPayload :(UnfiledAdException * _Nonnull)error { 
    if(self.onInterstitialLoadFailedCallback) {
        self.onInterstitialLoadFailedCallback([adPayload.placementName UTF8String], [error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onInterstitialLoaded:(UnfiledAdPayload * _Nonnull)adPayload { 
    if(self.onInterstitialLoadedCallback) {
        self.onInterstitialLoadedCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onInterstitialShowFailed:(UnfiledAdPayload * _Nonnull)adPayload :(UnfiledAdException * _Nonnull)error { 
    if(self.onInterstitialShowFailedCallback) {
        self.onInterstitialShowFailedCallback([adPayload.placementName UTF8String], [error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onInterstitialShown:(UnfiledAdPayload * _Nonnull)adPayload { 
    if(self.onInterstitialShownCallback) {
        self.onInterstitialShownCallback([adPayload.placementName UTF8String]);
    }
}

@end

