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
                            .Add("edition", Serializer.ToJson(id.edition));
                    })
                    .FromJson((json, type) => {
                        return new PieceId(
                            Serializer.FromJson<PieceType>(json["type"]),
                            Serializer.FromJson<PieceEdition>(json["edition"])
                        );
                    })
            );
        }
        
        public PieceType type;
        public PieceEdition edition;

        public PieceId(PieceType type, PieceEdition edition)
        {
            this.type = type;
            this.edition = edition;
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
            return type == other.type && edition == other.edition;
        }

        public override bool Equals(object obj)
        {
            return obj is PieceId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) type * 397) ^ (int) edition;
            }
        }

        public override string ToString()
        {
            return typeof(PieceId).FullName + $"({type}, {edition})";
        }
    }
}