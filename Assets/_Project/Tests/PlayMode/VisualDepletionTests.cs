using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using FrontierPioneers.Gameplay.InventorySystem;
using FrontierPioneers.Gameplay.Resources;
using NUnit.Framework;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace FrontierPioneers
{
    [TestFixture]
    public class VisualDepletionTests
    {
        const int MineralChunksCount = 5;
        const float SingleChunkPercentage = 1f / MineralChunksCount;
        
        private GameObject _visualObject;
        private VisualDepletion _visualDepletion;
        private ResourceNode _resourceNode;
        
        [SetUp]
        public void Setup()
        {
            GameObject resourceNodeObject = new GameObject("Resource Node Object");
            resourceNodeObject.SetActive(false);
            ResourceNode node = resourceNodeObject.AddComponent<ResourceNode>();
            _resourceNode = node;
            
            ResourceNodeConfigSO resourceNodeConfigSO = ScriptableObject.CreateInstance<ResourceNodeConfigSO>();
            resourceNodeConfigSO.basicResource = ScriptableObject.CreateInstance<ItemSO>();
            
            typeof(ResourceNode).GetField("resourceNodeConfig", BindingFlags.NonPublic | BindingFlags.Instance)
                                ?.SetValue(node, resourceNodeConfigSO);
            
            _visualObject = new GameObject("Visual");
            _visualObject.transform.SetParent(resourceNodeObject.transform);
            
            GameObject chunksTransform = new GameObject("Chunks");
            chunksTransform.transform.SetParent(_visualObject.transform);
            
            for (int i = 0; i < MineralChunksCount; i++)
            {
                GameObject chunk = new GameObject($"Mineral Chunk {i}");
                chunk.transform.SetParent(chunksTransform.transform);
                chunk.AddComponent<MeshRenderer>();
            }
            
            _visualDepletion = _visualObject.AddComponent<VisualDepletion>();
            
            
            resourceNodeObject.SetActive(true);
            _visualDepletion.enabled = true;
            _visualObject.SetActive(true);
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_visualObject);
        }
        
        [Test]
        public void Awake_InitializesAllResourcePieces()
        {
            Assert.AreEqual(MineralChunksCount, _visualDepletion.AllResourcePiecesCount);
            Assert.AreEqual(MineralChunksCount, _visualDepletion.VisibleResourcePiecesCount);
            Assert.AreEqual(1f, _visualDepletion.CurrentPercentage);
        }

        [Test]
        public void Awake_InitializesAllRenderersAsEnabled()
        {
            foreach(var renderer in _visualObject.GetComponentsInChildren<MeshRenderer>())
            {
                Assert.IsTrue(renderer.enabled);
            }
        }
        
        [Test]
        public void VisualDepleteAll_SetsAllRenderersAsDisabled()
        {
            _visualDepletion.VisualDepleteAll();
            
            foreach (var renderer in _visualObject.GetComponentsInChildren<MeshRenderer>())
            {
                Assert.IsFalse(renderer.enabled);
            }
        }
        
        [Test]
        public void VisualDepleteAll_SetsCorrectPercentage()
        {
            _visualDepletion.VisualDepleteAll();
            
            Assert.AreEqual(0f, _visualDepletion.CurrentPercentage);
        }

        [Test]
        public void VisualDepleteAll_SetsCorrectVisibleResourcePiecesCount()
        {
            _visualDepletion.VisualDepleteAll();
            
            Assert.AreEqual(0, _visualDepletion.VisibleResourcePiecesCount);
        }
        
        [Test]
        public void VisualDepleteAll_GetsCorrectAllResourcePiecesCount()
        {
            _visualDepletion.VisualDepleteAll();
            
            Assert.AreEqual(MineralChunksCount, _visualDepletion.AllResourcePiecesCount);
        }
        
        [Test]
        public void SetVisualPercentage_InvalidPercentage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _visualDepletion.SetVisualPercentage(-0.0001f));
            Assert.Throws<ArgumentException>(() => _visualDepletion.SetVisualPercentage(1.0001f));
        }
        
        [Test]
        public void SetVisualPercentage_SetsCorrectVisibleResourcePiecesCount()
        {
            _visualDepletion.SetVisualPercentage(4 * SingleChunkPercentage);
            Assert.AreEqual(4, _visualDepletion.VisibleResourcePiecesCount);
        }
        
        [Test]
        public void SetVisualPercentage_WhenLittleDeletedSetsCorrectVisibleResourcePiecesCount()
        {
            _visualDepletion.SetVisualPercentage((MineralChunksCount-0.01f) * SingleChunkPercentage);
            Assert.AreEqual(MineralChunksCount - 1, _visualDepletion.VisibleResourcePiecesCount);
        }
        
        [Test]
        public void SetVisualPercentage_WhenSomeLeftSetsCorrectVisibleResourcePiecesCount()
        {
            _visualDepletion.SetVisualPercentage(0.01f * SingleChunkPercentage);
            Assert.AreEqual(1, _visualDepletion.VisibleResourcePiecesCount);
        }
        
    }
}
