using Gmc.Core;
using Xunit;

namespace Gmc.Core.Tests;

public class CreditsCheckTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), "gmc-credits-" + Guid.NewGuid().ToString("N"));

    public CreditsCheckTests() => Directory.CreateDirectory(_root);
    public void Dispose() { try { Directory.Delete(_root, true); } catch (IOException) { } }

    void Write(string content, string name = CreditsCheck.FileName) => File.WriteAllText(Path.Combine(_root, name), content);

    [Fact] public void MissingFile_IsInvalid() => Assert.False(CreditsCheck.IsValid(_root));

    [Fact] public void FileWithMarker_IsValid()
    {
        Write("Copyright & Asset Credits (版权)");
        Assert.True(CreditsCheck.IsValid(_root));
    }

    [Fact] public void FileWithoutMarker_IsInvalid()
    {
        Write("credits without the marker");
        Assert.False(CreditsCheck.IsValid(_root));
    }

    [Fact] public void EmptyFile_IsInvalid()
    {
        Write("");
        Assert.False(CreditsCheck.IsValid(_root));
    }

    [Fact] public void EmptyMarker_OnlyNeedsTheFile()
    {
        Write("anything");
        Assert.True(CreditsCheck.IsValid(_root, marker: ""));
    }

    [Fact] public void FolderWithThatName_IsInvalid()
    {
        Directory.CreateDirectory(Path.Combine(_root, CreditsCheck.FileName));
        Assert.False(CreditsCheck.IsValid(_root));
    }

    [Fact] public void MissingGameFolder_IsInvalidAndDoesNotThrow()
        => Assert.False(CreditsCheck.IsValid(Path.Combine(_root, "does-not-exist")));

    [Fact] public void HugeFile_IsInvalid()
    {
        File.WriteAllBytes(Path.Combine(_root, CreditsCheck.FileName), new byte[(1 << 20) + 1]);
        Assert.False(CreditsCheck.IsValid(_root));
    }
}
