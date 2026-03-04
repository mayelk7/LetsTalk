namespace LetsTalk.Shared.ModelsDto;
public class ParticipantLiveKit
{
    public int Identity { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
    public bool CanPublish { get; set; }
    public bool CanSubscribe { get; set; }
    public bool IsSharingScreen { get; set; }
    public string AvatarUrl { get; set; }
    public string Username { get; set; }
}
