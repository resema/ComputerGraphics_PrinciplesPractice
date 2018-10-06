using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Threading;

namespace spike3D
{
    class BufferCache
    {
        private delegate void BlockOperation(BlockHolder bh);

        /// <summary>
        /// Simple helper class to hold a little metadata on blocks in the cache.
        /// This is responsible for reading and writing data to disk
        /// </summary>
        private class BlockHolder
        {
            public byte[] Data
            {
                get { return m_data; }
            }

            public int BlockNum
            {
                get { return m_blockNum; }
            }

            /// <summary>
            /// Sets the data for the block and sets the dirty bit to true
            /// </summary>
            public void SetData(byte[] data)
            {
                Debug.Assert(data.Length == c_blockSize);
                m_isDirty = true;
                m_data = data;
            }

            /// <summary>
            /// Writes out the current block and reads in the given block number from disk.
            /// This is a blocking call.
            /// </summary>
            public void PopulateHolderFromDisk(int blockNum)
            {
                WriteDataToDisk();
                diskblockread(m_data, blockNum);
                m_blockNum = blockNum;
            }

            /// <summary>
            /// Writes out the current block and sets the data 
            /// of the new block from the passed in data.
            /// This could block on disk.
            /// </summary>
            public void PopulateHolderWithData(int blockNum, byte[] data)
            {
                WriteDataToDisk();
                m_blockNum = blockNum;
                SetData(data);
            }

            /// <summary>
            /// Writes the block to disk if it is dirty, and cleans the dirty bit.
            /// This could block on disk.
            /// </summary>
            private void WriteDataToDisk()
            {
                // if the data is dirty write it back to disk
                if (m_isDirty)
                {
                    diskblockwrite(m_data, BlockNum);
                }
                m_isDirty = false;
            }

            private int m_blockNum = -1;
            private byte[] m_data;
            private bool m_isDirty;
        }

        /// <summary>
        /// Sets up a buffer cache with a set number of blocks to keep in memory.
        /// </summary>
        public BufferCache()
        {
            for (int ii = 0; ii < c_cacheBlocks; ii++)
            {
                m_cache.Add(new BlockHolder());
            }
        }

        /// <summary>
        /// Performs a block operation passing in two delegates.  The first
        /// peforms the read if it was a cache hit and the second performs
        /// the read if it was a cache miss.
        /// </summary>
        public int ReadBlock(byte[] data, int blockNum)
        {
            return PerformBlockOperation(blockNum,
                delegate(BlockHolder bh)
                {
                    data = bh.Data;
                },
                delegate(BlockHolder bh) 
                {
                    bh.PopulateHolderFromDisk(blockNum);
                    data = bh.Data;
                });
        }

        /// <summary>
        /// Performs a block operation passing in two delegates.  The first
        /// peforms the read if it was a cache hit and the second performs
        /// the write if it was a cache miss.
        /// </summary>
        public int WriteBlock(byte[] data, int blockNum)
        {
            return PerformBlockOperation(blockNum,
                delegate(BlockHolder bh)
                {
                    Debug.Assert(blockNum == bh.BlockNum, "double check we are writing the block to the correct holder");
                    bh.SetData(data);
                },
                delegate(BlockHolder bh)
                {
                    bh.PopulateHolderWithData(blockNum, data);
                });
        }


        /// <summary>
        /// Looks to see if the block is in the cache.  if it is, it performs 
        /// the cache hit operation on the data.  If the block was not in the cache
        /// it pulls the block holder out of the cache so that no other thread can
        /// access the block.  It then performs the blocking read/write disk i/o
        /// after giving up all locks.  Finally it gets the locks again to insert
        /// the new block back into the cache
        /// </summary>
        /// <param name="cacheHit">Operation to perform if the block is in cache (cannot block)</param>
        /// <param name="chacheMiss">Operation to perform if the block is not in cache (can block)</param>
        private int PerformBlockOperation(int blockNum,
            BlockOperation cacheHit, BlockOperation chacheMiss)
        {
            BlockHolder bh = null;
            bool success = false;

            do
            {
                lock (m_cache)
                {
                    bh = GetHolderForBlockNumInCache(blockNum);
                    if (bh == null)
                    {
                        // the block is not in cache
                        while (m_cache.Count == 0)
                        {
                            // condition wait on the cache until there is a block in
                            // the cache we can read into.  This is a very rare case. 
                            // It would only happen if more threads were waiting on
                            // the cache than there were spaces for blocks in the cache
                        }
                        // remove the oldest thing on the cache.
                        lock (m_diskQueue)
                        {
                            if (!m_diskQueue.Contains(blockNum))
                            {
                                bh = GetOldestCacheItem();
                                m_diskQueue.Add(blockNum);
                            }
                            else
                            {
                                // someone else is already fetching this block from disk.
                                // After we release the lock
                            }
                        }
                    }
                    else
                    {
                        // the block is in the cache, so just perform the operation,
                        // and update it's position to maintain LRU semantics
                        cacheHit(bh);
                        UpdateLastUsedTime(bh);
                        success = true;
                    }
                }// unlock cache
                if (!success)
                {
                    if (bh == null)
                    {
                        // if we missed the cache (i.e. !success) and the holder is null
                        // it means another thread is fetching that block from disk.
                        lock (m_diskQueue)
                        {
                            while (m_diskQueue.Contains(blockNum))
                            {
                                // condition wait on m_diskQueue
                            }
                        }
                        // If we get here the block should be in the cache (at least for now)
                        // success is still false, so we'll loop in this method and hopefully
                        // get a cache hit, or else work on getting the block again.
                    }
                    else
                    {
                        // The holder is now isolated, so we can make all the blocking calls we want 
                        chacheMiss(bh);
                        lock (m_cache)
                        {
                            lock (m_diskQueue)
                            {
                                AddMostRecentlyUsed(bh);
                                m_diskQueue.Remove(bh.BlockNum);
                                // broadcast on the condition variable associated with m_diskQueue
                            }
                            success = true;
                        }
                    }
                }
            } while (!success);

            return 0;
        }


        // The following methods provide cache operations that abstract away the implementation of the cache.

        /// <summary>
        /// Gets the block holder for the given blockNum if it is in the cache, otherwise returns null.
        /// </summary>
        private BlockHolder GetHolderForBlockNumInCache(int blockNum)
        {
            BlockHolder toRet = null;
            foreach (BlockHolder bh in m_cache)
            {
                if (bh.BlockNum == blockNum)
                {
                    toRet = bh;
                    break;
                }
            }
            return toRet;
        }

        /// <summary>
        /// Updates the given block holer to be the most recently used item in the cache
        /// </summary>
        private void UpdateLastUsedTime(BlockHolder bh)
        {
            m_cache.Remove(bh);
            AddMostRecentlyUsed(bh);
        }

        /// <summary>
        /// Adds the given holder to the cache as the most recently used item
        /// </summary>
        private void AddMostRecentlyUsed(BlockHolder bh)
        {
            m_cache.Add(bh);
        }

        /// <summary>
        /// Removes the oldest item in the cache and returns it.
        /// </summary>
        private BlockHolder GetOldestCacheItem()
        {
            Debug.Assert(m_cache.Count > 0, "There have to be items in the cache");
            BlockHolder toRet = m_cache[0];
            m_cache.Remove(toRet);
            return toRet;
        }


        private List<BlockHolder> m_cache = new List<BlockHolder>();
        private HashSet<int> m_diskQueue = new HashSet<int>();

        public const int c_blockSize = 512;
        public const int c_cacheBlocks = 256;
    }
}
