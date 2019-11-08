using System.Collections;
using GatheringChess.Playground;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace GatheringChess.Editor.Tests
{
    public class PieceIdTest
    {

        [Test]
        public void ItCanBeCompared()
        {
            PieceId def = PieceId.Default;
            PieceId some = new PieceId(
                PieceType.Bishop,
                PieceColor.Black,
                PieceEdition.ManRay
            );
            PieceId nul = null;
            
            Assert.IsTrue(PieceId.Default == def);
            Assert.IsTrue(def == PieceId.Default);
            Assert.IsFalse(def == some);
            
            Assert.IsTrue(def != some);
            Assert.IsFalse(def != PieceId.Default);
            
            Assert.IsTrue(nul == null);
            Assert.IsTrue(null == nul);
            Assert.IsFalse(nul == def);
            Assert.IsFalse(def == nul);
            
            Assert.IsFalse(nul != null);
            Assert.IsFalse(null != nul);
            Assert.IsTrue(nul != def);
            Assert.IsTrue(def != nul);
        }
    }
}