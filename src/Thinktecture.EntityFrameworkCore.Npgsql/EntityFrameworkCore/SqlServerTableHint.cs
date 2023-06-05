using Microsoft.EntityFrameworkCore.Storage;

namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// SQL Server table hints.
/// </summary>
public class NpgsqlTableHint : ITableHint, IEquatable<NpgsqlTableHint>
{
   /// <summary>
   /// NOEXPAND
   /// </summary>
   public static readonly NpgsqlTableHint NoExpand = new("NOEXPAND");

   /// <summary>
   /// FORCESCAN
   /// </summary>
   public static readonly NpgsqlTableHint ForceScan = new("FORCESCAN");

   /// <summary>
   /// FORCESEEK
   /// </summary>
   public static readonly NpgsqlTableHint ForceSeek = new("FORCESEEK");

   /// <summary>
   /// HOLDLOCK
   /// </summary>
   public static readonly NpgsqlTableHint HoldLock = new("HOLDLOCK");

   /// <summary>
   /// NOLOCK
   /// </summary>
   public static readonly NpgsqlTableHint NoLock = new("NOLOCK");

   /// <summary>
   /// NOWAIT
   /// </summary>
   public static readonly NpgsqlTableHint NoWait = new("NOWAIT");

   /// <summary>
   /// PAGLOCK
   /// </summary>
   public static readonly NpgsqlTableHint PagLock = new("PAGLOCK");

   /// <summary>
   /// READCOMMITTED
   /// </summary>
   public static readonly NpgsqlTableHint ReadCommitted = new("READCOMMITTED");

   /// <summary>
   /// READCOMMITTEDLOCK
   /// </summary>
   public static readonly NpgsqlTableHint ReadCommittedLock = new("READCOMMITTEDLOCK");

   /// <summary>
   /// READPAST
   /// </summary>
   public static readonly NpgsqlTableHint ReadPast = new("READPAST");

   /// <summary>
   /// READUNCOMMITTED
   /// </summary>
   public static readonly NpgsqlTableHint ReadUncommitted = new("READUNCOMMITTED");

   /// <summary>
   /// REPEATABLEREAD
   /// </summary>
   public static readonly NpgsqlTableHint RepeatableRead = new("REPEATABLEREAD");

   /// <summary>
   /// ROWLOCK
   /// </summary>
   public static readonly NpgsqlTableHint RowLock = new("ROWLOCK");

   /// <summary>
   /// SERIALIZABLE
   /// </summary>
   public static readonly NpgsqlTableHint Serializable = new("SERIALIZABLE");

   /// <summary>
   /// SNAPSHOT
   /// </summary>
   public static readonly NpgsqlTableHint Snapshot = new("SNAPSHOT");

   /// <summary>
   /// TABLOCK
   /// </summary>
   public static readonly NpgsqlTableHint TabLock = new("TABLOCK");

   /// <summary>
   /// TABLOCKX
   /// </summary>
   public static readonly NpgsqlTableHint TabLockx = new("TABLOCKX");

   /// <summary>
   /// UPDLOCK
   /// </summary>
   public static readonly NpgsqlTableHint UpdLock = new("UPDLOCK");

   /// <summary>
   /// XLOCK
   /// </summary>
   public static readonly NpgsqlTableHint XLock = new("XLOCK");

   /// <summary>
   /// SPATIAL_WINDOW_MAX_CELLS
   /// </summary>
   public static NpgsqlTableHint Spatial_Window_Max_Cells(int value)
   {
      return new($"SPATIAL_WINDOW_MAX_CELLS = {value}");
   }

   /// <summary>
   /// INDEX(name)
   /// </summary>
   public static NpgsqlTableHint Index(string name)
   {
      return new IndexTableHint(name);
   }

   private readonly string _value;

   private NpgsqlTableHint(string value)
   {
      _value = value ?? throw new ArgumentNullException(nameof(value));
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return ReferenceEquals(this, obj) || (obj is NpgsqlTableHint other && Equals(other));
   }

   /// <inheritdoc />
   public bool Equals(NpgsqlTableHint? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return _value == other._value;
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return _value.GetHashCode();
   }

   /// <inheritdoc />
   public override string ToString()
   {
      return _value;
   }

   /// <inheritdoc />
   public virtual string ToString(ISqlGenerationHelper sqlGenerationHelper)
   {
      return _value;
   }

   private sealed class IndexTableHint : NpgsqlTableHint, IEquatable<IndexTableHint>
   {
      private readonly string _name;

      public IndexTableHint(string name)
         : base($"INDEX({name})")
      {
         _name = name;
      }

      public override bool Equals(object? obj)
      {
         return ReferenceEquals(this, obj) || (obj is IndexTableHint other && Equals(other));
      }

      public bool Equals(IndexTableHint? other)
      {
         if (ReferenceEquals(null, other))
            return false;
         if (ReferenceEquals(this, other))
            return true;
         return base.Equals(other) && _name == other._name;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(base.GetHashCode(), _name);
      }

      public override string ToString(ISqlGenerationHelper sqlGenerationHelper)
      {
         return $"INDEX({sqlGenerationHelper.DelimitIdentifier(_name)})";
      }
   }
}
