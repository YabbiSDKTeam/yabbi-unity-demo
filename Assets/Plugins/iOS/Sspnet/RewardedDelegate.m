#if defined(__has_include) && __has_include("UnityInterface.h")
#import "UnityInterface.h"
#endif
#import "RewardedDelegate.h"

@implementation RewardedDelegate

- (void)onRewardedLoaded {
    if(self.onRewardedVideoLoadedCallback) {
        self.onRewardedVideoLoadedCallback();
    }
}

- (void)onRewardedLoadFailed:(UnfiledAdException * _Nonnull)error {
    if(self.onRewardedVideoLoadFailedCallback) {
        self.onRewardedVideoLoadFailedCallback([error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onRewardedShown {
    if(self.onRewardedVideoShownCallback) {
        self.onRewardedVideoShownCallback();
    }
}

- (void)onRewardedShowFailed:(UnfiledAdException * _Nonnull)error {
    if(self.onRewardedVideoShowFailedCallback) {
        self.onRewardedVideoShowFailedCallback([error.localizedDescription UTF8String], [error.message UTF8String], [error.caused UTF8String]);
    }
}

- (void)onRewardedClosed {
    if(self.onRewardedVideoClosedCallback) {
        self.onRewardedVideoClosedCallback();
    }
}

- (void)onRewardedFinished {
    if(self.onRewardedVideoFinishedCallback) {
        self.onRewardedVideoFinishedCallback();
    }
}

@end
