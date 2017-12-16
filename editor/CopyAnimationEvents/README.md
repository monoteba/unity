# Copy Animation Events

Editor script to copy animation events from one clip to another. 

The script also works for imported models (like FBX files). 

**Important;** When using on models the target clip is matched by name. This means that if you have two animation clips with the same name on the same model, both will get the events assigned.

Copy the script to an **Editor** folder. Open the tool by going to **Assets > Copy Animation Events** in the menu.

Copying more than one results in the first clip on the left is copied to the first on the right, the second on the left to the second on the right etc. If a clip is missing on either side on any given row, it is skipped.

![Copy Animation Events](CopyAnimationEvents_preview.jpg)