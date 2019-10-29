using System;
using System.Collections.Generic;
using LightJson;
using Unisave.Serialization;

namespace GatheringChess
{
    /// <summary>
    /// ID of a single chess piece that can be owned by a player
    /// </summary>
    public struct PieceId : IEquatable<PieceId>
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
                            .Add("edition", Serializer.ToJson(id.edition))
                            .Add("color", Serializer.ToJson(id.color));
                    })
                    .FromJson((json, type) => {
                        return new PieceId(
                            Serializer.FromJson<PieceType>(json["type"]),
                            Serializer.FromJson<PieceEdition>(json["edition"]),
                            Serializer.FromJson<PieceColor>(json["color"])
                        );
                    })
            );
        }
        
        public readonly PieceType type;
        public readonly PieceEdition edition;
        public readonly PieceColor color;

        public PieceId(PieceType type, PieceEdition edition, PieceColor color)
        {
            this.type = type;
            this.edition = edition;
            this.color = color;
        }
        
        public static bool operator ==(PieceId a, PieceId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PieceId a, PieceId b)
        {
            return !(a == b);
        }

        public bool Equals(PieceId other)
        {
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