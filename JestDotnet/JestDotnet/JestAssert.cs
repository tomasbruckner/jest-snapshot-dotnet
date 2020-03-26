using System;
using System.Runtime.CompilerServices;
using JestDotnet.Core;
using JestDotnet.Core.Exceptions;

namespace JestDotnet
{
    public static class JestAssert
    {
        public static void ShouldMatchSnapshot(
            object actual,
            string hint = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = ""
        )
        {
            var path = SnapshotResolver.CreatePath(sourceFilePath, memberName, hint);
            var snapshot = SnapshotResolver.GetSnapshotData(path);

            if (string.IsNullOrEmpty(snapshot))
            {
                SnapshotUpdater.TryUpdateMissingSnapshot(path, actual);
                return;
            }

            var (isValid, message) = (ValueTuple<bool, string>) SnapshotComparer.CompareSnapshots(snapshot, actual);
            if (!isValid)
            {
                SnapshotUpdater.TryUpdateSnapshot(path, actual, message);
            }
        }

        public static void ShouldMatchInlineSnapshot(dynamic actual, string inlineSnapshot)
        {
            var (isValid, message) =
                (ValueTuple<bool, string>) SnapshotComparer.CompareSnapshots(inlineSnapshot, actual);
            if (!isValid)
            {
                throw new SnapshotMismatch(message);
            }
        }

        public static void ShouldMatchObject(dynamic actual, dynamic expected)
        {
            var (isValid, message) = (ValueTuple<bool, string>) SnapshotComparer.CompareSnapshots(expected, actual);
            if (!isValid)
            {
                throw new SnapshotMismatch(message);
            }
        }
    }
}
