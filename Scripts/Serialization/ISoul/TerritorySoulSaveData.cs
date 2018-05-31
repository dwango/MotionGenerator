using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class TerritorySoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<TerritorySoulSaveData>
    {
        [Key(0)] public List<double> TerritoryCenter { get; set; }

        public TerritorySoulSaveData()
        {
            
        }

        public TerritorySoulSaveData(List<double> territoryCenter)
        {
            TerritoryCenter = territoryCenter;
        }

        public TerritorySoulSaveData(Vector territoryCenter) : this(territoryCenter.ToList())
        {
        }

        public ISoul Instantiate()
        {
            return new TerritorySoul(new DenseVector(TerritoryCenter.ToArray()));
        }
    }
}