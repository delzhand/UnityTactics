using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_TimelineEvent : TimelineEvent {
    public string MessageLabel;
    public string MessageText;
    public Timeline Timeline;
    public Message Message;

    public void Init(Timeline timeline, float delay, string messageLabel, string messageText)
    {
        SetTime(timeline.CompileTime, delay);
        Timeline = timeline;
        MessageLabel = messageLabel;
        MessageText = messageText;
    }

    private void Update()
    {
        // The message is created at the same time as this is started, so this condition indicates the message has been cleared
        if (Started && Message == null)
        {
            Timeline.Play = true;
        }
    }

    public override void Play()
    {
        base.Play();
        Message = Message.CreateNew(MessageLabel, MessageText, 3f);
        Engine.AssignControl(Message);
        Timeline.Play = false; // Pause the timeline while this message is displayed
    }
}
