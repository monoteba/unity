#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <UIKIT/UIKit.h>

@interface AudioSessionManager : NSObject

+ (void)setupAudioSession;
+ (void)routeChange:(NSNotification *)notification;
+ (void)interruption:(NSNotification *)notification;
+ (void)changeRoute;
+ (BOOL)recordPermission;

@end

// Public function called from Unity
void AudioSessionManager_SetupAVAudioSession();
BOOL AudioSessionManager_RequestRecordPermission();
BOOL AudioSessionManager_RecordPermissionStatus();