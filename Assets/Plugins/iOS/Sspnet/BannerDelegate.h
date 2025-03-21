#import <Foundation/Foundation.h>
#import <SspnetCore/SspnetCore-Swift.h>

typedef void (*BannerCallbacks) (const char *);
typedef void (*BannerFailCallbacks) (const char *, const char *, const char *, const char *);

@interface BannerDelegate : NSObject <UnfiledBannerDelegate>

@property (assign, nonatomic) BannerCallbacks onBannerLoadedCallback;
@property (assign, nonatomic) BannerFailCallbacks onBannerLoadFailedCallback;
@property (assign, nonatomic) BannerCallbacks onBannerShownCallback;
@property (assign, nonatomic) BannerFailCallbacks onBannerShowFailedCallback;
@property (assign, nonatomic) BannerCallbacks onBannerClosedCallback;
@property (assign, nonatomic) BannerCallbacks onBannerImpressionCallback;

@end
