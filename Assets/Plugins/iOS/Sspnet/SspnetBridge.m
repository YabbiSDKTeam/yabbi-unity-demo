#if defined(__has_include) && __has_include("UnityAppController.h")
#import "UnityAppController.h"
#endif
#import <Foundation/Foundation.h>
#import <SspnetCore/SspnetCore-Swift.h>
#import "InterstitialDelegate.h"
#import "RewardedDelegate.h"


UIViewController *SspnetRootViewController(void) {
#if defined(__has_include) && __has_include("UnityAppController.h")
    return ((UnityAppController *)[UIApplication sharedApplication].delegate).rootViewController;
#else
    return NULL;
#endif
}


void SspnetInitialize(const char *publisherID) {
    [SspnetCoreSDK initializeWithPublisherID:[NSString stringWithUTF8String:publisherID]];
}

BOOL SspnetIsInitialized(void) {
    return [SspnetCoreSDK isInitialized];
}

void SspnetLoadAd(int adType, const char *placementName){
    NSString * _Nonnull placement = [NSString stringWithUTF8String:placementName];
    [SspnetCoreSDK loadAd:adType :placement];
}

BOOL SspnetCanLoadAd(int adType, const char *placementName){
    NSString * _Nonnull placement = [NSString stringWithUTF8String:placementName];
    return [SspnetCoreSDK canLoadAd:adType :placement];
}

BOOL SspnetIsAdLoaded(int adType, const char *placementName){
    NSString * _Nonnull placement = [NSString stringWithUTF8String:placementName];
    return [SspnetCoreSDK isAdLoaded:adType :placement];
}

void SspnetShowAd(int adType, const char *placementName) {
    NSString * _Nonnull placement = [NSString stringWithUTF8String:placementName];
    [SspnetCoreSDK showAdWithAdType:adType placementName:placement rootViewController:SspnetRootViewController()];
}

void SspnetDestroyAd(int adType, const char *placementName){
    NSString * _Nonnull placement = [NSString stringWithUTF8String:placementName];
    return [SspnetCoreSDK destroyAd:adType :placement];
}

void SspnetDestroyAdByType(int adType){
    return [SspnetCoreSDK destroyAd:adType];
}

void SspnetSetCustomParams(const char *key, const char *value){
    return [SspnetCoreSDK setCustomParams:[NSString stringWithUTF8String:key] :[NSString stringWithUTF8String:value]];
}

void SspnetSetUserConsent(BOOL hasConsent){
    [SspnetCoreSDK setUserConsent:hasConsent];
}

void SspnetEnableDebug(BOOL enable){
    [SspnetCoreSDK enableDebug:enable];
}

BOOL SspnetHasUserConsent(void) {
    return [SspnetCoreSDK hasUserConsent];
}

NSString* SspnetGetSdkVersion(void) {
    return [SspnetCoreSDK sdkVersion];
}

static InterstitialDelegate *InterstitialDelegateInstance;
void SspnetSetInterstitialDelegate(
                                  InterstitialCallbacks onInterstitialLoaded,
                                  InterstitialFailCallbacks onInterstitialLoadFailed,
                                  InterstitialCallbacks onInterstitialShown,
                                  InterstitialFailCallbacks onRewardedVideoShownFailed,
                                  InterstitialCallbacks onInterstitialClosed
                                  ) {
    
    InterstitialDelegateInstance = [InterstitialDelegate new];
    
    InterstitialDelegateInstance.onInterstitialLoadedCallback = onInterstitialLoaded;
    InterstitialDelegateInstance.onInterstitialLoadFailedCallback = onInterstitialLoadFailed;
    InterstitialDelegateInstance.onInterstitialShownCallback = onInterstitialShown;
    InterstitialDelegateInstance.onInterstitialShowFailedCallback = onRewardedVideoShownFailed;
    InterstitialDelegateInstance.onInterstitialClosedCallback = onInterstitialClosed;
    
    
    [SspnetCoreSDK setInterstitialDelegate:InterstitialDelegateInstance];
}

static RewardedDelegate *RewardedVideoDelegateInstance;
void SspnetSetRewardedDelegate(
                              RewardedVideoCallbacks onRewardedVideoLoaded,
                              RewardedVideoFailCallbacks onRewardedVideoLoadFailed,
                              RewardedVideoCallbacks onRewardedVideoShown,
                              RewardedVideoFailCallbacks onRewardedVideoShownFailed,
                              RewardedVideoCallbacks onRewardedVideoClosed,
                              RewardedVideoCallbacks onRewardedVideoFinished
                              ) {
    
    RewardedVideoDelegateInstance = [RewardedDelegate new];
    
    RewardedVideoDelegateInstance.onRewardedVideoLoadedCallback = onRewardedVideoLoaded;
    RewardedVideoDelegateInstance.onRewardedVideoLoadFailedCallback = onRewardedVideoLoadFailed;
    RewardedVideoDelegateInstance.onRewardedVideoShownCallback = onRewardedVideoShown;
    RewardedVideoDelegateInstance.onRewardedVideoShowFailedCallback = onRewardedVideoShownFailed;
    RewardedVideoDelegateInstance.onRewardedVideoClosedCallback = onRewardedVideoClosed;
    RewardedVideoDelegateInstance.onRewardedVideoFinishedCallback = onRewardedVideoFinished;
    
    [SspnetCoreSDK setRewardedDelegate:RewardedVideoDelegateInstance];
}
