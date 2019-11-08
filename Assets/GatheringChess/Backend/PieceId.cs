using System;
using System.Collections.Generic;
using System.Linq;
using LightJson;
using Unisave.Serialization;
using UnityEngine;
using Object = System.Object;

namespace GatheringChess
{
    /// <summary>
    /// ID of a single chess piece that can be owned by a player
    /// </summary>
    public class PieceId : IEquatable<PieceId>
    {
        static PieceId()
        {
            // TODO: fix unisave so that it can serialize a type like this
            
            Serializer.SetExactTypeSerializer(
                typeof(PieceId),
                new LambdaTypeSerializer()
                    .ToJson(obj => {
                        PieceId id = (PieceId)obj;
                        return new JsonObject()
                            .Add("type", Serializer.ToJson(id.type))
                            .Add("color", Serializer.ToJson(id.color))
                            .Add("edition", Serializer.ToJson(id.edition));
                    })
                    .FromJson((json, type) => {
                        return new PieceId(
                            Serializer.FromJson<PieceType>(json["type"]),
                            Serializer.FromJson<PieceColor>(json["color"]),
                            Serializer.FromJson<PieceEdition>(json["edition"])
                        );
                    })
            );
        }
        
        public readonly PieceType type;
        public readonly PieceEdition edition;
        public readonly PieceColor color;
        
        /// <summary>
        /// Default piece to use instead of null
        /// </summary>
        public static readonly PieceId Default = new PieceId(
            default(PieceType), default(PieceColor), default(PieceEdition)
        );
        
        public PieceId(PieceType type, PieceColor color, PieceEdition edition)
        {
            this.type = type;
            this.edition = edition;
            this.color = color;
        }

        public Sprite LoadSprite()
        {
            Sprite[] atlas = Resources.LoadAll<Sprite>($"Pieces/{edition}");
            string name = "" + color.GetLetter() + type.GetLetter();
            return atlas.Single(s => s.name == name);
        }
        
        public static bool operator ==(PieceId a, PieceId b)
        {
            // null == null T  ;  null == val F
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            
            // val == null F  ;  val == val CMP
            return a.Equals(b);
        }

        public static bool operator !=(PieceId a, PieceId b)
        {
            return !(a == b);
        }

        public bool Equals(PieceId other)
        {
            if (ReferenceEquals(other, null))
                return false;
            
            return type == other.type
                   && edition == other.edition
                   && color == other.color;
        }

        public override bool Equals(object obj)
        {
            return obj is PieceId other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) type;
                hashCode = (hashCode * 397) ^ (int) edition;
                hashCode = (hashCode * 397) ^ (int) color;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return typeof(PieceId).FullName + $"({type}, {edition})";
        }
    }
}