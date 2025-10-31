using UnityEngine;

public class RegularSound : WagBehaviour, ISoundConfig
{
    [SerializeField] public SoundInstruction Instruction;

    public virtual AudioClip Clip => Instruction.Clip;
    public AudioType Type => Instruction.Type;
    public virtual string Id => Instruction.Clip.name;
    public virtual float Volume => Instruction.Volume;
    public virtual float Pitch => Instruction.Pitch;
}