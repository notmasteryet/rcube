using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenCVProxy.Interop;

namespace OpenCVProxy
{
    public class CvSeqCollection<T> : IList<T>, IDisposable
    {
        IntPtr cvSeq;
        bool isReadOnly;

        public T this[int index]
        {
            get
            {
                IntPtr item = CxCore.cvGetSeqElem(cvSeq, index);
                return (T)Marshal.PtrToStructure(item, typeof(T));
            }
            set
            {
                CheckReadOnly();
                using (StructureSafeMemoryBox<T> box = CreateMemoryBox(value))
                {
                    CxCore.cvSeqRemove(cvSeq, index);
                    CxCore.cvSeqInsert(cvSeq, index, box.Pointer);
                }
            }
        }

        public CvSeqCollection(IntPtr cvSeq, bool isReadOnly)
        {
            this.cvSeq = cvSeq;
            this.isReadOnly = isReadOnly;
        }

        public CvSeqCollection(int flags, IntPtr storage)
        {
            this.isReadOnly = false;
            this.cvSeq = CxCore.cvCreateSeq(flags,
                CvTypeSizes.CvSeqSize, Marshal.SizeOf(typeof(T)), storage);
        }

        private void CheckReadOnly()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("Collection is in read-only mode");
        }

        private StructureSafeMemoryBox<T> CreateMemoryBox(T item)
        {
            StructureSafeMemoryBox<T> box = new StructureSafeMemoryBox<T>();
            box.Value = item;
            return box;
        }

        public T[] ToArray()
        {
            T[] array = new T[Count];
            CopyTo(array, 0);
            return array;
        }

#region "ICollection<T> implementation"

        public int Count
        {
            get
            {
                const int CountOffset = 6 * 4;
                return Marshal.ReadInt32(cvSeq, CountOffset);
            }
        }

        public virtual bool IsReadOnly
        {
            get { return true; }
        }

        public virtual void Add(T item)
        {
            CheckReadOnly();
            using (StructureSafeMemoryBox<T> box = CreateMemoryBox(item))
            {
                CxCore.cvSeqPush(cvSeq, box.Pointer);
            }
        }

        public virtual void Insert(int index, T item)
        {
            CheckReadOnly();
            using (StructureSafeMemoryBox<T> box = CreateMemoryBox(item))
            {
                CxCore.cvSeqInsert(cvSeq, index, box.Pointer);
            }
        }

        public virtual bool Remove(T item)
        {
            CheckReadOnly();
            int i = IndexOf(item);
            if (i >= 0)
            {
                RemoveAt(i);
                return true;
            }
            else
                return false;
        }

        public virtual void RemoveAt(int index)
        {
            CheckReadOnly();
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException();

            CxCore.cvSeqRemove(cvSeq, index);
        }

        public virtual void Clear()
        {
            CheckReadOnly();
            CxCore.cvClearSeq(cvSeq);
        }

        public virtual int IndexOf(T item)
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                if (item.Equals(this[i])) return i;
            }
            return -1;
        }

        public virtual bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        public virtual void Dispose()
        {
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }
#endregion
    }
}
