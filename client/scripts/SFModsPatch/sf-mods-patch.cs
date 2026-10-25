// SF Mods patch: proximity tracker (players/animals counter in the info bar) disabled
function sfInfoBarProximity()
{
}
function sfInfoBarProximityStart()
{
}
cancel($sfInfoBarProximity);
if (isObject(SFGuiInfoProximity)) SFGuiInfoProximity.setText("");
if (isObject(SFGuiInfoProximityLarge)) SFGuiInfoProximityLarge.setText("");
function sfHideProximityLabels()
{
    %names = "SFGuiInfoProximity SFGuiInfoProximityLarge";
    for (%k = 0; %k < 2; %k++)
    {
        %value = getWord(%names, %k);
        if (!isObject(%value)) continue;
        %value.setVisible(0);
        %parent = %value.getParent();
        if (!isObject(%parent)) continue;
        for (%i = 0; %i < %parent.getCount(); %i++)
        {
            %c = %parent.getObject(%i);
            if (%c.getClassName() $= "GuiTextCtrl" && %c.getValue() $= "Proximity") %c.setVisible(0);
        }
    }
}
sfHideProximityLabels();
exec("mod/sf-mods-localchat.cs");
exec("mod/sf-mods-tabs.cs");
