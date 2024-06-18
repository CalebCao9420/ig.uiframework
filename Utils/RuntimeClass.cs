using System;
using System.ComponentModel;
using UnityEngine;

namespace IG.Module.UI{
    /// <summary>
    /// Runtime class.
    /// </summary>
    public class RuntimeClass{
        Type[]                             types               = new Type[]{ typeof(ParticleSystem), typeof(ParticleSystemRenderer), typeof(MeshCollider) };
        private static StringConverter     stringConverter     = new StringConverter();
        private static BooleanConverter    booleanConverter    = new BooleanConverter();
        private static Int64Converter      int64Converter      = new Int64Converter();
        private static Int32Converter      int32Converter      = new Int32Converter();
        private static CollectionConverter collectionConverter = new CollectionConverter();
        private static EnumConverter       enumConverter       = new EnumConverter(typeof(EnumConverter));
        private static ArrayConverter      arrayConverter      = new ArrayConverter();
        public RuntimeClass(){ }
    }
}