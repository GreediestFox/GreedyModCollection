// GM helper (client side, GreedyFox). The chat box cannot carry GM commands (it uses its own chat protocol), so these console
// functions send them to the server's GMCommands mod through the same path that works: commandToServer('LocalChatMessage', "!...").
// Console (~) usage:
//   gm("whoami");           gm("help");
//   gm("give 241 50");      gm("give <itemTypeId> [quantity] [quality] [durability]");
//   gm("announce Hello!");
//   gmgive(241, 50);        gmsay("Hello!");
function gm(%text)
{
   commandToServer('LocalChatMessage', "!" @ %text);
}

function gmgive(%type, %qty, %quality, %durability)
{
   commandToServer('LocalChatMessage', "!give" SPC %type SPC %qty SPC %quality SPC %durability);
}

function gmsay(%text)
{
   commandToServer('LocalChatMessage', "!announce" SPC %text);
}

exec("mod/GMHelper/gmpanel.cs");
