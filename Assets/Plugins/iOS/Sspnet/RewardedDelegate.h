#import <Foundation/Foundation.h>
#import <SspnetCore/SspnetCore-Swift.h>

typedef void (*RewardedVideoCallbacks) (const char *);
typedef void (*RewardedVideoFailCallbacks) (const char *, const char *, const char *, const char *);

@interface RewardedDelegate : NSObject <UnfiledRewardedDelegate>

@property (assign, nonatomic) RewardedVideoCallbacks onRewardedVideoLoadedCallback;
@property (assign, nonatomic) RewardedVideoFailCallbacks onRewardedVideoLoadFailedCallback;
@property (assign, nonatomic) RewardedVideoCallbacks onRewardedVideoShownCallback;
@property (assign, nonatomic) RewardedVideoFailCallbacks onRewardedVideoShowFailedCallback;
@property (assign, nonatomic) RewardedVideoCallbacks onRewardedVideoClosedCallback;
@property (assign, nonatomic) RewardedVideoCallbacks onRewardedVideoFinishedCallback;

@end
