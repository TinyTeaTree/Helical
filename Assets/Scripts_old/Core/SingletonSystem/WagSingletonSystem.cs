using System.Collections.Generic;

public class WagSingletonSystem
{
    public static List<IResetable> Resetables { get; }= new List<IResetable>();

}