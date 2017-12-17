#import "AudioSessionManager.h"

@implementation AudioSessionManager {
}

+ (void)setupAudioSession {
    // Set options for AVAudioSession object
    AVAudioSession *session = [AVAudioSession sharedInstance];
    NSError *error;

    [session
            setCategory:AVAudioSessionCategoryPlayAndRecord
            withOptions:AVAudioSessionCategoryOptionDefaultToSpeaker
                  error:&error
    ];

    if (error)
        NSLog(@"Error setting up AVAudioSession: %@", error);

    // Set the session as the currently active session
    [session setActive:YES error:&error];

    if (error)
        NSLog(@"Error setting AVAudioSession as the active session");

    [self addObservers];

    /*
     * Bluetooth output is currently only available using HFP, when using the AVAudioSessionCategoryPlayAndRecord
     * category. Furthermore, it is not possible to force input through the built-in microphone and only output to
     * A2DP or LE. Output to A2DP or LE is only possible when not recording any input.
     */
}

+ (void)routeChange:(NSNotification *)notification {
    NSInteger reason = [[notification.userInfo valueForKey:AVAudioSessionRouteChangeReasonKey] integerValue];
    NSLog(@"AVAudioSession: Route change notification! (%li)", (long) reason);

    switch (reason) {
        case AVAudioSessionRouteChangeReasonNewDeviceAvailable: // 1
        case AVAudioSessionRouteChangeReasonOldDeviceUnavailable: // 2
        case AVAudioSessionRouteChangeReasonNoSuitableRouteForCategory: // 7
            // Check and change route if necessary
            [self changeRoute];
            break;

        case AVAudioSessionRouteChangeReasonUnknown: // 0
        case AVAudioSessionRouteChangeReasonWakeFromSleep: // 6
            [self removeObservers];
            [self setupAudioSession];
            break;

        case AVAudioSessionRouteChangeReasonCategoryChange: // 3
        case AVAudioSessionRouteChangeReasonOverride: // 4
        case AVAudioSessionRouteChangeReasonRouteConfigurationChange: // 8
        default:
            // No action taken
            break;
    }
}

+ (void)interruption:(NSNotification *)notification {
    NSLog(@"AVAudioSession: Interruption notification!");

    NSInteger reason = [[notification.userInfo valueForKey:AVAudioSessionInterruptionTypeKey] integerValue];

    if (reason == AVAudioSessionInterruptionTypeBegan) {
        [self removeObservers];
    } else if (reason == AVAudioSessionInterruptionTypeEnded) {
        [self setupAudioSession];
    }
}

+ (void)changeRoute {
    // Handle route change
    NSLog(@"change route...");
    for (AVAudioSessionPortDescription *description in [[[AVAudioSession sharedInstance] currentRoute] outputs]) {
        NSLog(@"...for");
        NSArray *inputs = [[AVAudioSession sharedInstance] availableInputs];

        if ([[description portType] isEqualToString:AVAudioSessionPortBuiltInReceiver] ||
                [[description portType] isEqualToString:AVAudioSessionPortBuiltInSpeaker]) {

            /*for (AVAudioSessionPortDescription *port in inputs) {
                if ([port.portType isEqualToString:AVAudioSessionPortBuiltInMic]) {
                    [[AVAudioSession sharedInstance] setPreferredInput:port error:nil];
                    break;
                } else if ([port.portType isEqualToString:AVAudioSessionPortBuiltInReceiver]) {
                    [[AVAudioSession sharedInstance] setPreferredInput:port error:nil];
                    break;
                }
            }*/

            [[AVAudioSession sharedInstance] overrideOutputAudioPort:AVAudioSessionPortOverrideSpeaker error:nil];

            break;

        } else {

            /*for (AVAudioSessionPortDescription *port in inputs) {
                if ([port.portType isEqualToString:AVAudioSessionPortHeadsetMic]) {
                    [[AVAudioSession sharedInstance] setPreferredInput:port error:nil];
                    break;
                } else if ([port.portType isEqualToString:AVAudioSessionPortLineIn]) {
                    [[AVAudioSession sharedInstance] setPreferredInput:port error:nil];
                    break;
                }
            }*/

            [[AVAudioSession sharedInstance] overrideOutputAudioPort:AVAudioSessionPortOverrideNone error:nil];

            break;

        }
    }
}

+ (BOOL)recordPermission {
    // Request permission to record audio
    __block BOOL permission;

    [[AVAudioSession sharedInstance] requestRecordPermission:^(BOOL response) {
        permission = response;
    }];

    return permission;
}

+ (void)addObservers {
    // Register for audio session route change notifications
    [[NSNotificationCenter defaultCenter]
            addObserver:self
               selector:@selector(routeChange:)
                   name:AVAudioSessionRouteChangeNotification
                 object:[AVAudioSession sharedInstance]
    ];

    // Register for audio session interruption
    [[NSNotificationCenter defaultCenter]
            addObserver:self
               selector:@selector(interruption:)
                   name:AVAudioSessionInterruptionNotification
                 object:[AVAudioSession sharedInstance]
    ];
}

+ (void)removeObservers {

    [[NSNotificationCenter defaultCenter]
            removeObserver:self
                      name:AVAudioSessionRouteChangeNotification
                    object:[AVAudioSession sharedInstance]
    ];

    [[NSNotificationCenter defaultCenter]
            removeObserver:self
                      name:AVAudioSessionInterruptionNotification
                    object:[AVAudioSession sharedInstance]
    ];
}

@end

// Public function called from Unity
void AudioSessionManager_SetupAVAudioSession() {
    if ([UIDevice currentDevice].userInterfaceIdiom == UIUserInterfaceIdiomPhone) {
        // Only do the special audio session if iPhone
        [AudioSessionManager removeObservers];
        [AudioSessionManager setupAudioSession];
    }
}

BOOL AudioSessionManager_RequestRecordPermission() {
    return [AudioSessionManager recordPermission];
}

BOOL AudioSessionManager_RecordPermissionStatus() {
    NSUInteger response = [[AVAudioSession sharedInstance] recordPermission];
    if (response == AVAudioSessionRecordPermissionGranted) {
        return true;
    } else {
        return false;
    }
}