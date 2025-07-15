using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using HotFix_UI;
using Unity.Collections;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace XFramework
{
    /// <summary>
    /// 音频类型
    /// </summary>
    public enum AudioType
    {
        /// <summary>
        /// 背景音乐
        /// </summary>
        BGM,

        /// <summary>
        /// 音效
        /// </summary>
        SFX,
    }

    public sealed class AudioManager : CommonObject
    {
        /// <summary>
        /// bgm音量 
        /// </summary>
        private const string BGMValue = "BGMVALUE";

        /// <summary>
        /// bgm静音
        /// </summary>
        private const string BGMMUTE = "BGMMUTE";

        /// <summary>
        /// sfx音量
        /// </summary>
        private const string SFXValue = "SFXVALUE";

        /// <summary>
        /// sfx静音
        /// </summary>
        private const string SFXMUTE = "SFXMUTE";

        public static AudioManager _instance;

        public static AudioManager Instance => _instance;

        private Dictionary<AudioType, AudioSource> dictAudio = new Dictionary<AudioType, AudioSource>();

        /// <summary>
        /// 音频类型 -> 音频Clip的key
        /// </summary>
        private Dictionary<AudioType, string> dictClipKeys = new Dictionary<AudioType, string>();

        public static List<string> banks { get; } = new List<string>()
        {
            "SoundEffects.bank",
            "SoundEffects.strings.bank",
            "Master.bank",
            "Master.strings.bank",
            "BGM.bank",
            "BGM.strings.bank"
        };

        public static List<string> RunTimeAudioEvent { get; } = new List<string>()
        {
            "NormalCollideSound",
            "PickGemSound",
            "Warning",
            "UIBGM"
        };

        private const string BGMPath = "bus:/BGM";
        private const string SoundEffectsPath = "bus:/SoundEffects";

        async protected override void Init()
        {
            base.Init();

            _instance = this;


            // var global = Common.Instance.Get<Global>();
            // var gameRoot = global.GameRoot;
            //
            // var bgm = gameRoot.gameObject.AddComponent<AudioSource>();
            // var sfx = gameRoot.gameObject.AddComponent<AudioSource>();
            //
            // bgm.loop = true;
            // bgm.playOnAwake = false;
            // sfx.loop = false;
            // sfx.playOnAwake = false;
            //
            // if (PlayerPrefsHelper.TryGetFloat(BGMValue, out float bgmVolume)) 
            //     bgm.volume = bgmVolume;
            //
            // if (PlayerPrefsHelper.TryGetBool(BGMMUTE, out bool bgmMute))
            //     bgm.mute = bgmMute;
            //
            // if (PlayerPrefsHelper.TryGetFloat(SFXValue, out float sfxVolume))
            //     sfx.volume = sfxVolume;
            //
            // if (PlayerPrefsHelper.TryGetBool(SFXMUTE, out bool sfxMute))
            //     sfx.mute = sfxMute;
            //

            // dictAudio.Add(AudioType.BGM, bgm);
            // dictAudio.Add(AudioType.SFX, sfx);
            await LoadAudio();

            SetFModAllMute(true);
            //AudioManager.Instance.SetFModSFXMute(true);
        }

        async UniTask LoadAudio()
        {
            foreach (var bank in banks)
            {
                var textAsset = await ResourcesManager.LoadAssetAsync<TextAsset>(bank);
                RuntimeManager.LoadBank(textAsset, false);
                Log.Debug($"音频源banks:{bank} 已加载", Color.green);
            }


            // RuntimeManager.

            // if (RuntimeManager.HasBankLoaded("BGM"))
            // {
            // RuntimeManager.StudioSystem.getBankList(out var banks);
            // foreach (var VARIABLE in banks)
            // {
            //     if (VARIABLE.isValid())
            //     {
            //         VARIABLE.getPath(out var path);
            //         Log.Error($"{path}");
            //     }
            // }
            // }

            //RuntimeManager.AnyBankLoading()
            //Master.
            //RuntimeManager.ge
            // if (!RuntimeManager.IsInitialized)
            // {
            //     Log.Error($"!IsInitialized");
            //     return;
            // }

            // List<string> clipsList = new List<string>();
            // //clipsList.Add("NormalCollideSound");
            // clipsList.Add("PickGemSound");
            // RuntimeManager.GetEventDescription("event:/PickGemSound");
            //
            //
            // foreach (var VARIABLE in clipsList)
            // {
            //     var newstr = $"event:/{VARIABLE}";
            // }
        }

        public NativeHashMap<FixedString128Bytes, EventDescription> InitRunTimeAudio()
        {
            var tbaudio = ConfigManager.Instance.Tables.Tbaudio;
            var audioClips =
                new NativeHashMap<FixedString128Bytes, EventDescription>(tbaudio.DataList.Count, Allocator.Persistent);


            try
            {
                foreach (var audio in tbaudio.DataList)
                {
                    var newstr = $"event:/New/{audio.group}";
                    var result = RuntimeManager.StudioSystem.getEvent(newstr, out var eventDescription);

                    if (eventDescription.isValid())
                    {
                        audioClips.TryAdd(audio.group, eventDescription);
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                throw;
            }


            return audioClips;
        }

        protected override void Destroy()
        {
            base.Destroy();
            // foreach (var audioSource in dictAudio.Values)
            // {
            //     GameObject.DestroyImmediate(audioSource, true);
            // }

            dictAudio.Clear();
            dictClipKeys.Clear();

            _instance = null;
        }
        /*
        #region Old

        public bool TryGetAudioSource(AudioType audioType, out AudioSource audioSource)
        {
            return dictAudio.TryGetValue(audioType, out audioSource);
        }

        /// <summary>
        /// 获取音量
        /// </summary>
        /// <param name="audioType"></param>
        /// <returns></returns>
        public float GetVolume(AudioType audioType)
        {
            if (TryGetAudioSource(audioType, out var source))
                return source.volume;

            return 0;
        }

        /// <summary>
        /// 获取是否静音
        /// </summary>
        /// <param name="audioType"></param>
        /// <returns></returns>
        public bool GetMute(AudioType audioType)
        {
            if (TryGetAudioSource(audioType, out var source))
                return source.mute;

            return true;
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="audioType"></param>
        public void Stop(AudioType audioType)
        {
            if (TryGetAudioSource(audioType, out var source))
                source.Stop();
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="audioType"></param>
        /// <param name="volume"></param>
        public void SetVolume(AudioType audioType, float volume)
        {
            if (TryGetAudioSource(audioType, out var source))
            {
                source.volume = volume;
                switch (audioType)
                {
                    case AudioType.BGM:
                        PlayerPrefsHelper.SetFloat(BGMValue, volume, true);
                        break;
                    case AudioType.SFX:
                        PlayerPrefsHelper.SetFloat(SFXMUTE, volume, true);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 设置静音
        /// </summary>
        /// <param name="audioType"></param>
        /// <param name="mute"></param>
        public void SetMute(AudioType audioType, bool mute)
        {
            if (TryGetAudioSource(audioType, out var source))
            {
                source.mute = mute;
                switch (audioType)
                {
                    case AudioType.BGM:
                        //PlayerPrefsHelper.SetBool(BGMMUTE, mute, true);
                        break;
                    case AudioType.SFX:
                        //PlayerPrefsHelper.SetBool(SFXMUTE, mute, true);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 设置循环播放
        /// </summary>
        /// <param name="audioType"></param>
        /// <param name="loop"></param>
        public void SetLoop(AudioType audioType, bool loop)
        {
            if (TryGetAudioSource(audioType, out var source))
            {
                source.loop = loop;
            }
        }

        /// <summary>
        /// bgm音量
        /// </summary>
        /// <returns></returns>
        public float GetBgmVolume()
        {
            return GetVolume(AudioType.BGM);
        }

        /// <summary>
        /// sfx音量
        /// </summary>
        /// <returns></returns>
        public float GetSFXVolume()
        {
            return GetVolume(AudioType.SFX);
        }

        /// <summary>
        /// bgm静音
        /// </summary>
        /// <returns></returns>
        public bool GetBgmMute()
        {
            return GetMute(AudioType.BGM);
        }

        /// <summary>
        /// sfx静音
        /// </summary>
        /// <returns></returns>
        public bool GetSFXMute()
        {
            return GetMute(AudioType.SFX);
        }

        /// <summary>
        /// 停止bgm
        /// </summary>
        public void StopBgm()
        {
            Stop(AudioType.BGM);
        }

        /// <summary>
        /// 停止sfx
        /// </summary>
        public void StopSFX()
        {
            Stop(AudioType.SFX);
        }

        /// <summary>
        /// 设置bgm音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetBgmVolume(float volume)
        {
            this.SetVolume(AudioType.BGM, volume);
        }


        /// <summary>
        /// 设置bgm循环播放
        /// </summary>
        /// <param name="loop"></param>
        public void SetBgmLoop(bool loop)
        {
            this.SetLoop(AudioType.BGM, loop);
        }

        /// <summary>
        /// 设置sfx音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetSFXVolume(float volume)
        {
            this.SetVolume(AudioType.SFX, volume);
        }


        /// <summary>
        /// 设置sfx循环播放
        /// </summary>
        /// <param name="loop"></param>
        public void SetSFXLoop(bool loop)
        {
            this.SetLoop(AudioType.SFX, loop);
        }


        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="audioType"></param>
        /// <param name="audioKey"></param>
        /// <param name="ignoreIfPlaying">如果正在播放音频，则忽略这一次播放</param>
        public void PlayAudio(AudioType audioType, string audioKey, bool ignoreIfPlaying)
        {
            if (TryGetAudioSource(audioType, out var source))
            {
                if (source.isPlaying && ignoreIfPlaying)
                    return;

                source.Stop();

                if (dictClipKeys.TryGetValue(audioType, out var key))
                {
                    if (key == audioKey)
                    {
                        source.Play();
                        return;
                    }

                    if (source.clip != null)
                    {
                        ResourcesManager.ReleaseAsset(source.clip);
                        source.clip = null;
                    }
                }

                source.clip = ResourcesManager.LoadAsset<AudioClip>(this, audioKey);
                source.Play();
                dictClipKeys[audioType] = audioKey;
            }
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="audioType"></param>
        /// <param name="audioKey"></param>
        public void PlayAudio(AudioType audioType, string audioKey)
        {
            PlayAudio(audioType, audioKey, false);
        }

        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="audioKey"></param>
        /// <param name="ignoreIfPlaying">如果正在播放音频，则忽略这一次播放</param>
        public void PlayBgmAudio(string audioKey, bool ignoreIfPlaying)
        {
            PlayAudio(AudioType.BGM, audioKey, ignoreIfPlaying);
        }

        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="audioKey"></param>
        public void PlayBgmAudio(string audioKey)
        {
            PlayAudio(AudioType.BGM, audioKey);
        }

        /// <summary>
        /// 播放SFX
        /// </summary>
        /// <param name="audioKey"></param>
        /// <param name="ignoreIfPlaying">如果正在播放音频，则忽略这一次播放</param>
        // public void PlaySFXAudio(string audioKey, bool ignoreIfPlaying)
        // {
        //     PlayAudio(AudioType.SFX, audioKey, ignoreIfPlaying);
        // }
        //
        // /// <summary>
        // /// 播放SFX
        // /// </summary>
        // /// <param name="audioKey"></param>
        // public void PlaySFXAudio(string audioKey)
        // {
        //     PlayAudio(AudioType.SFX, audioKey);
        // }

        #endregion
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioId"></param>
        public void PlayFModAudio(int audioId)
        {
            if (audioId == 0) return;

            //Log.Debug($"try play:audio.id:{audioId}", Color.green);
            var tbaudio = ConfigManager.Instance.Tables.Tbaudio;
            var audio = tbaudio.GetOrDefault(audioId);
            if (audio == null)
            {
                Log.Error($"音频id不存在表中:{audioId}");
                return;
            }

            //var audio = tbaudio.Get(audioId);

            //Log.Debug($"try play:audioId:{audio.id} audio.group:{audio.group}", Color.green);

            var newstr = $"event:/New/{audio.group}";
            var result = RuntimeManager.StudioSystem.getEvent(newstr, out var clip);

            if (clip.isValid())
            {
                Log.Debug($"audioId:{audio.id} audio.group:{audio.group}", Color.green);
                clip.createInstance(out var instance);

                //instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
                instance.setVolume(audio.volume / 10000f);
                //instance.setProperty(EVENT_PROPERTY.SCHEDULE_LOOKAHEAD, 1); // 1 = loop, 0 = no loop
                instance.start();


                instance.release();

                //instance.
                //AudioManager.Instance.pl
            }
            else
            {
                Log.Error($"音频组不存在 audioId:{audio.id} audio.group:{audio.group}");
            }
        }

        /// <summary>
        /// 停止audio bgm
        /// </summary>
        /// <param name="audioId"></param>
        public void ClearFModBgmAudio()
        {
            StopFModAudio(2101);
            StopFModAudio(2102);
            StopFModAudio(2103);
            StopFModAudio(1151);
            StopFModAudio(1152);
            StopFModAudio(1153);
            StopFModAudio(1154);
        }

        /// <summary>
        /// 停止audio
        /// </summary>
        /// <param name="audioId"></param>
        public void StopFModAudio(int audioId, STOP_MODE stopMode = STOP_MODE.IMMEDIATE)
        {
            var tbaudio = ConfigManager.Instance.Tables.Tbaudio;
            var audio = tbaudio.GetOrDefault(audioId);
            if (audio == null)
            {
                Log.Error($"音频id不存在表中:{audioId}");
                return;
            }

            try
            {
                var newstr = $"event:/New/{audio.group}";
                var result = RuntimeManager.StudioSystem.getEvent(newstr, out var clip);

                if (clip.isValid())
                {
                    clip.getInstanceList(out var list);
                    foreach (var ins in list)
                    {
                        //ins.get
                        ins.stop(stopMode);

                        ins.release();
                    }

                    // clip.createInstance(out var instance);
                    // //instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
                    // instance.setVolume(audio.volume / 10000f);
                    // //instance.setProperty(EVENT_PROPERTY.SCHEDULE_LOOKAHEAD, 1); // 1 = loop, 0 = no loop
                    // instance.start();
                    //
                    //
                    // instance.release();

                    //instance.
                    //AudioManager.Instance.pl
                }
            }
            catch (EventNotFoundException)
            {
                //throw new EventNotFoundException(path);
            }
        }

        /// <summary>
        /// 设置所有声音静音
        /// </summary>
        /// <param name="mute"></param>
        public void SetFModAllMute(bool mute)
        {
            RuntimeManager.StudioSystem.getBus(SoundEffectsPath, out var bus1);
            bus1.setMute(mute);
            RuntimeManager.StudioSystem.getBus(BGMPath, out var bus2);
            bus2.setMute(mute);
            //this.SetMute(AudioType.SFX, mute);
        }


        /// <summary>
        /// 设置sfx静音
        /// </summary>
        /// <param name="mute"></param>
        public void SetFModSFXMute(bool mute)
        {
            //Log.Debug($"SetFModSFXMute {mute}");
            RuntimeManager.StudioSystem.getBus(SoundEffectsPath, out var bus);

            bus.setMute(mute);
        }

        /// <summary>
        /// 设置bgm静音
        /// </summary>
        /// <param name="mute"></param>
        public void SetFModBgmMute(bool mute)
        {
            RuntimeManager.StudioSystem.getBus(BGMPath, out var bus);
            bus.setMute(mute);
        }


        /// <summary>
        /// 设置所有声音静音
        /// </summary>
        /// <param name="mute"></param>
        public void StopFModSFX(bool stop = false)
        {
            RuntimeManager.StudioSystem.getBus(SoundEffectsPath, out var bus1);

            bus1.setPaused(stop);
        }
    }
}