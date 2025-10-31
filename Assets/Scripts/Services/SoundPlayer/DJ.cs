using UnityEngine;

namespace Services
{
    public static class DJ
    {
        public static ISoundPlayerService SoundPlayer { get; set; }

        public static void Play(IDesignSound soundDesign)
        {
            SoundPlayer.Play(soundDesign);
        }

        public static void Play(IDesignSound soundDesign, Vector3 position)
        {
            SoundPlayer.Play(soundDesign, position);
        }

        public static void Play(IDesignSound soundDesign, Transform transform)
        {
            SoundPlayer.Play(soundDesign, transform);
        }

        public static void Stop(IDesignSound soundDesign)
        {
            SoundPlayer.Stop(soundDesign);
        }

        public static SoundPlayer GetPlayer(IDesignSound soundDesign)
        {
            return SoundPlayer.GetPlayer(soundDesign);
        }

        public static void PlayMusic(IDesignSound soundDesign)
        {
            SoundPlayer.PlayMusic(soundDesign);
        }

        public static void EndMusic(IDesignSound soundDesign)
        {
            SoundPlayer.EndMusic(soundDesign);
        }
    }
}