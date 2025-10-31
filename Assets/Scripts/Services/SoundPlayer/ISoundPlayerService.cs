using Core;
using UnityEngine;

namespace Services
{
    public interface ISoundPlayerService : IService
    {
        /// <summary>
        /// This will play a sound as is without fading or any logic.
        ///
        /// Unless Looping is enabled, this will also stop itself when over.
        ///
        /// In case Looping is enabled. make sure to Stop the sound eventually with the Stop() API
        /// </summary>
        void Play(IDesignSound soundDesign);

        /// <summary>
        /// Just like regular Play but with a calculation of volume based on proximity
        /// </summary>
        void Play(IDesignSound soundDesign, Vector3 position);

        /// <summary>
        /// Just like regular Play but with a continuous calculation of volume base on proximity to transform
        /// </summary>
        void Play(IDesignSound soundDesign, Transform transform);
        
        /// <summary>
        /// Will look for all playing clips from this design to stop them
        /// </summary>
        void Stop(IDesignSound soundDesign);

        /// <summary>
        /// For advanced custom usages and requirements
        /// </summary>
        SoundPlayer GetPlayer(IDesignSound soundDesign);

        /// <summary>
        /// This will play this sound in the Music Channel, and will Fade it.
        /// In case there are currently other PlayMusic(), it will fade them out and fade them back in when this is over or stopped.
        ///
        /// This can be stopped only with EndMusic() API, and not with regular Stop()
        ///
        /// If another Sound is PlayMusic() called, it will fade this music out until it is over or ended.
        /// </summary>
        void PlayMusic(IDesignSound soundDesign);

        /// <summary>
        /// Parallel to PlayMusic()
        /// </summary>
        void EndMusic(IDesignSound soundDesign);
    }
}