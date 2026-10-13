// FeudalTools Tradepost client mod v1.1 loader (2026-10-10). The shipped cmod.cs expects a LiFx client framework that the
// stock client does not have, so this loader execs the client part directly. The mod itself (compiled .dso only, third party)
// is not in this repository: put cmod.cs.dso and tpclient.cs.dso from the FeudalTools download into mod/Tradepost/ and
// exec this file from the end of art/materials.cs (where the other client mods are exec'd).
exec("mod/Tradepost/tpclient.cs");
if (isPackage(TradingPostMod))
   activatePackage(TradingPostMod);
