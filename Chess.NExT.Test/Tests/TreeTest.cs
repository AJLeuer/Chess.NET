using System;
using Chess.Utility;
using FluentAssertions;
using NUnit.Framework;

namespace Chess.Test.Tests
{
    public static class TreeTest
    {
        [Test]
        public static void ShouldCountTotalNodesAccurately()
        {
            var tree = new TreeNode<bool> { false, true, false };
            var subtree0 = new TreeNode<bool> { false, true, false };
            var subtree1 = new TreeNode<bool> { false, true, false };

            tree.Children[0].Add(subtree0);
            tree.Children[2].Add(subtree1);

            tree.Count.Should().Be(12);
        }
        
        [Test]
        public static void ClearShouldReduceCountToOne()
        {
            var tree = new TreeNode<bool> { false, true, false };

            tree.Children[1].AddChildren(true, false);
            
            tree.Clear();

            tree.Count.Should().Be(1);
        }
        
        [Test]
        public static void ContainsShouldBeTrueIfContained()
        {
            var tree = new TreeNode<UInt16> { 1, 1, 1 };
            var subtree0 = new TreeNode<UInt16> { 1, 42, 1 };
            var subtree1 = new TreeNode<UInt16> { 1, 1, 1 };

            tree.Children[0].Add(subtree0);
            tree.Children[2].Add(subtree1);

            Assert.True(tree.Contains(42));
        }
        
        [Test]
        public static void ContainsShouldBeFalseIfNotContained()
        {
            var tree = new TreeNode<UInt16> { 1, 1, 1 };
            var subtree0 = new TreeNode<UInt16> { 1, 777, 1 };
            var subtree1 = new TreeNode<UInt16> { 1, 1, 1 };

            tree.Children[0] = subtree0;
            tree.Children[2] = subtree1;

            Assert.False(tree.Contains(42));
        }
    }
}