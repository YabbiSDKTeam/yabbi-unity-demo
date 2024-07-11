#if defined(__has_include) && __has_include("UnityInterface.h")
#import "UnityInterface.h"
#endif
#import "RewardedDelegate.h"

@implementation RewardedDelegate

- (void)onRewardedClosed:(UnfiledAdPayload * _Nonnull)adPayload { 
    if(self.onRewardedVideoClosedCallback) {
        self.onRewardedVideoClosedCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onRewardedLoadFailed:(UnfiledAdPayload * _Nonnull)adPayload :(UnfiledAdException * _Nonnull)error { 
    if(self.onRewardedVideoLoadFailedCallback) {
        self.onRewardedVideoLoadFailedCallback([adPayload.placementName UTF8String], [error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onRewardedLoaded:(UnfiledAdPayload * _Nonnull)adPayload { 
    if(self.onRewardedVideoLoadedCallback) {
        self.onRewardedVideoLoadedCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onRewardedShowFailed:(UnfiledAdPayload * _Nonnull)adPayload :(UnfiledAdException * _Nonnull)error { 
    if(self.onRewardedVideoShowFailedCallback) {
        self.onRewardedVideoShowFailedCallback([adPayload.placementName UTF8String], [error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onRewardedShown:(UnfiledAdPayload * _Nonnull)adPayload { 
    if(self.onRewardedVideoShownCallback) {
        self.onRewardedVideoShownCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onRewardedVideoStarted:(UnfiledAdPayload * _Nonnull)adPayload {
    if(self.onRewardedVideoStartedCallback) {
        self.onRewardedVideoStartedCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onRewardedVideoCompleted:(UnfiledAdPayload * _Nonnull)adPayload {
    if(self.onRewardedVideoCompletedCallback) {
        self.onRewardedVideoCompletedCallback([adPayload.placementName UTF8String]);
    }
}

- (void)onUserRewarded:(UnfiledAdPayload * _Nonnull)adPayload { 
    if(self.onRewardedVideoUserRewardedCallback) {
        self.onRewardedVideoUserRewardedCallback([adPayload.placementName UTF8String]);
    }
}


@end
