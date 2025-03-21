#if defined(__has_include) && __has_include("UnityInterface.h")
#import "UnityInterface.h"
#endif
#import "BannerDelegate.h"

@implementation BannerDelegate

- (void)onBannerLoaded:(UnfiledAdPayload * _Nonnull)adPayload {
    if(self.onBannerLoadedCallback) {
        self.onBannerLoadedCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onBannerLoadFailed:(UnfiledAdPayload * _Nonnull)adPayload :(UnfiledAdException * _Nonnull)error {
    if(self.onBannerLoadFailedCallback) {
        self.onBannerLoadFailedCallback([adPayload.placementName UTF8String], [error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onBannerShown:(UnfiledAdPayload * _Nonnull)adPayload {
    if(self.onBannerShownCallback) {
        self.onBannerShownCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onBannerShowFailed:(UnfiledAdPayload * _Nonnull)adPayload :(UnfiledAdException * _Nonnull)error {
    if(self.onBannerShowFailedCallback) {
        self.onBannerShowFailedCallback([adPayload.placementName UTF8String], [error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onBannerClosed:(UnfiledAdPayload * _Nonnull)adPayload {
    if(self.onBannerClosedCallback) {
        self.onBannerClosedCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onBannerImpression:(UnfiledAdPayload * _Nonnull)adPayload {
    if(self.onBannerImpressionCallback) {
        self.onBannerImpressionCallback([adPayload.placementName UTF8String]);
    }
}

@end
