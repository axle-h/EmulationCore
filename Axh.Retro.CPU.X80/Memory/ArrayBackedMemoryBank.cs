﻿namespace Axh.Retro.CPU.X80.Memory
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Exceptions;
    using Axh.Retro.CPU.X80.Contracts.Memory;

    public class ArrayBackedMemoryBank : IReadableAddressSegment, IWriteableAddressSegment
    {
        protected readonly byte[] Memory;

        public ArrayBackedMemoryBank(IMemoryBankConfig memoryBankConfig)
        {
            this.Memory = new byte[memoryBankConfig.Length];
            this.Type = memoryBankConfig.Type;
            this.Address = memoryBankConfig.Address;
            this.Length = memoryBankConfig.Length;

            if (memoryBankConfig.State == null)
            {
                return;
            }

            if (memoryBankConfig.Length != memoryBankConfig.State.Length)
            {
                throw new MemoryConfigStateException(memoryBankConfig.Address, memoryBankConfig.Length, memoryBankConfig.State.Length);
            }
            Array.Copy(memoryBankConfig.State, 0, this.Memory, 0, memoryBankConfig.State.Length);
        }

        public MemoryBankType Type { get; }

        public ushort Address { get; }

        public ushort Length { get; }
        
        public byte ReadByte(ushort address)
        {
            return this.Memory[address];
        }

        public ushort ReadWord(ushort address)
        {
            // Construct 16 bit value in little endian.
            return BitConverter.ToUInt16(Memory, address);
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            var bytes = new byte[length];
            Array.Copy(Memory, address, bytes, 0, length);
            return bytes;
        }

        public void WriteByte(ushort address, byte value)
        {
            this.Memory[address] = value;
        }

        public void WriteWord(ushort address, ushort word)
        {
            var bytes = BitConverter.GetBytes(word);
            Memory[address] = bytes[0];
            Memory[address + 1] = bytes[1];
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            Array.Copy(values, 0, Memory, address, values.Length);
        }
    }
}
