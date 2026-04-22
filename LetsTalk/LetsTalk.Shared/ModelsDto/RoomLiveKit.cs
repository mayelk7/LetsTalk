namespace LetsTalk.Shared.ModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
public class RoomLiveKit
{
    public string Sid { get; set; }
    public string Name { get; set; }
    public int EmptyTimeout { get; set; }
    public int DepartureTimeout { get; set; }
    public int MaxParticipants { get; set; }
    public long CreationTime { get; set; }
    public long CreationTimeMs { get; set; }
    public string TurnPassword { get; set; }

    public List<Codec> EnabledCodecs { get; set; }

    public string Metadata { get; set; }
    public int NumParticipants { get; set; }
    public int NumPublishers { get; set; }
    public bool ActiveRecording { get; set; }
    public string Version { get; set; }

    public bool IsFull()
    {
        return NumParticipants >= MaxParticipants;
    }

    public bool IsRecording()
    {
        return ActiveRecording;
    }

    public List<Codec> GetAudioCodecs()
    {
        return EnabledCodecs?.Where(c => c.Mime.StartsWith("audio/")).ToList();
    }

    public List<Codec> GetVideoCodecs()
    {
        return EnabledCodecs?.Where(c => c.Mime.StartsWith("video/")).ToList();
    }

    public bool HasCodec(string codec)
    {
        return EnabledCodecs?.Any(c => c.Mime == codec) ?? false;
    }
}

public class Codec
{
    public string Mime { get; set; }
    public string FmtpLine { get; set; }
}

