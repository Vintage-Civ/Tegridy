﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

[assembly: ModInfo("Tegridy")]
namespace Tegridy
{
    public class TegridyPacket
    {
        public List<TegridyReport> Reports = new List<TegridyReport>();
    }

    public class ModTegridy : ModSystem
    {

    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class TegridyReport
    {
        public string Id;
        public string Name;
        public string Version;
        public string FileName;
        public int SourceType;
        public string Fingerprint;

        public static TegridyReport Create(Mod mod)
        {
            return new TegridyReport()
            {
                Id = mod.Info.ModID,
                Name = mod.Info.Name,
                Version = mod.Info.Version,
                FileName = mod.FileName,
                SourceType = (int)mod.SourceType,
                Fingerprint = ExtraMath.Sha256HashMod(mod)
            };
        }
    }

    public static class ExtraMath
    {
        static SHA256 twoFiftySixHasher = SHA256.Create();
        static SHA256 tfs => twoFiftySixHasher;

        public static string Sha256HashMod(Mod mod)
        {
            return mod.SourceType == EnumModSourceType.Folder ? Sha256HashFolder(mod.SourcePath) : Sha256HashFile(mod.SourcePath);
        }

        public static string Sha256HashFile(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                string str = GetString(tfs.ComputeHash(stream));
                tfs.Clear();
                return str;
            }
        }

        public static string Sha256HashFolder(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            Array.Sort(files);
            byte[] hash = new byte[0];

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];

                string relPath = file.Substring(folderPath.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relPath.ToLowerInvariant());

                tfs.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                byte[] contentBytes = File.ReadAllBytes(file);

                if (i < files.Length)
                {
                    tfs.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }
                else
                {
                    hash = tfs.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                }
            }

            tfs.Clear();
            return GetString(hash);
        }

        public static string GetString(byte[] hash)
        {
            string hashString = "";
            for (int i = 0; i < hash.Length; i++)
            {
                hashString += hash[i].ToString("x2");
            }
            return hashString;

        }

        public static string Sha256Hash(string value)
        {
            var hash = tfs.ComputeHash(Encoding.UTF8.GetBytes(value));
            tfs.Clear();
            return GetString(hash);
        }
    }
}