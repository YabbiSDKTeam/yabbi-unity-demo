#import <Foundation/Foundation.h>
#import <SspnetCore/SspnetCore-Swift.h>

typedef void (*InterstitialCallbacks) (void);
typedef void (*InterstitialFailCallbacks) (const char *, const char *, const char *);

@interface InterstitialDelegate : NSObject <UnfiledInterstitialDelegate>

@property (assign, nonatomic) InterstitialCallbacks onInterstitialLoadedCallback;
@property (assign, nonatomic) InterstitialFailCallbacks onInterstitialLoadFailedCallback;
@property (assign, nonatomic) InterstitialCallbacks onInterstitialShownCallback;
@property (assign, nonatomic) InterstitialFailCallbacks onInterstitialShowFailedCallback;
@property (assign, nonatomic) InterstitialCallbacks onInterstitialClosedCallback;

@end
