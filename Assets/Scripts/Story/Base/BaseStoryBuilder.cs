using DG.Tweening;
using GameFrameWork;
using UnityEngine;
using System;

public abstract class BaseStoryBuilder : IStoryBuilder
{
    public abstract void BuildStory();

    public void FadeBgm(int track, float endValue, float delay, float duration)
    {
        FadeBgmClip clip = FadeBgmClip.Create(endValue, delay, duration);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void PauseBgm(int track)
    {
        PauseBgmClip clip = PauseBgmClip.Create();
        StoryMgr.instance.AddClip(track, clip);
    }

    public void PlayBgm(int track, string assetPath, bool isLoop, float volume, float lerpTime, bool isForcePlay)
    {
        PlayBgmClip clip = PlayBgmClip.Create(assetPath, isLoop, volume, lerpTime, isForcePlay);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void PlaySe(int track, string assetPath, float volume = 1)
    {
        PlaySeClip clip = PlaySeClip.Create(assetPath, volume);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void ResumeBgm(int track)
    {
        ResumeBgmClip clip = ResumeBgmClip.Create();
        StoryMgr.instance.AddClip(track, clip);
    }

    public void RoleAnim(int track, int roleId, string animName, int playTime, float playSpeed)
    {
        RoleAnimClip clip = RoleAnimClip.Create(roleId, animName, playTime, playSpeed);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void RoleIdle(int track, int roleId, int dir)
    {
        RoleIdleClip clip = RoleIdleClip.Create(roleId, dir);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void RoleJump(int track, int roleId, Vector2 dir, float posZ)
    {
        RoleJumpClip clip = RoleJumpClip.Create(roleId, dir, posZ);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void RoleMove(int track, int roleId, Vector2 endPos)
    {
        RoleMoveClip clip = RoleMoveClip.Create(roleId, endPos);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void RolePos(int track, int roleId, Vector2 pos)
    {
        RolePosClip clip = RolePosClip.Create(roleId, pos);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void WaitTime(int track, float waitTime)
    {
        WaitTimeClip clip = WaitTimeClip.Create(waitTime);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void SceneObjectActive(int track, string objectName, bool isActive)
    {
        SceneObjectActiveClip clip = SceneObjectActiveClip.Create(objectName, isActive);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void RolePositionAnim(int track, int roleId, int animType, Vector3 endPos, float duration, Ease ease)
    {
        RolePositionAnimClip clip = RolePositionAnimClip.Create(roleId, animType, endPos, duration, ease);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void UIShowHide(int track, Type uiType, bool isActive)
    {
        UIShowHideClip clip = UIShowHideClip.Create(uiType, isActive);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void PauseEnemy(int track,int entityId) 
    {
        RolePauseClip clip = RolePauseClip.Create(entityId);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void ResumeEnemy(int track, int entityId)
    {
        RoleResumeClip clip = RoleResumeClip.Create(entityId);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void Callback(int track, GameFrameWorkAction action) 
    {
        CallbackClip clip = CallbackClip.Create(action);
        StoryMgr.instance.AddClip(track, clip);
    }

    public void Talk(int track, int talkId) 
    {
        TalkClip clip = TalkClip.Create(talkId);
        StoryMgr.instance.AddClip(track, clip);
    }
}