#import "iOSAppSettings.h"

void iOSAppSettings_OpenSettingsApp() {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
}