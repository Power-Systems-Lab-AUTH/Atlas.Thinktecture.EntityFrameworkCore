namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// SQL Server table hints.
/// </summary>
public class NpgsqlTableHintLimited : IEquatable<NpgsqlTableHintLimited>
{
   /// <summary>
   /// KEEPIDENTITY
   /// </summary>
   public static readonly NpgsqlTableHintLimited KeepIdentity = new("KEEPIDENTITY");

   /// <summary>
   /// KEEPDEFAULTS
   /// </summary>
   public static readonly NpgsqlTableHintLimited KeepDefaults = new("KEEPDEFAULTS");

   /// <summary>
   /// HOLDLOCK
   /// </summary>
   public static readonly NpgsqlTableHintLimited HoldLock = new("HOLDLOCK");

   /// <summary>
   /// IGNORE_CONSTRAINTS
   /// </summary>
   public static readonly NpgsqlTableHintLimited IgnoreConstraints = new("IGNORE_CONSTRAINTS");

   /// <summary>
   /// IGNORE_TRIGGERS
   /// </summary>
   public static readonly NpgsqlTableHintLimited IgnoreTriggers = new("IGNORE_TRIGGERS");

   /// <summary>
   /// NOLOCK
   /// </summary>
   public static readonly NpgsqlTableHintLimited NoLock = new("NOLOCK");

   /// <summary>
   /// NOWAIT
   /// </summary>
   public static readonly NpgsqlTableHintLimited NoWait = new("NOWAIT");

   /// <summary>
   /// PAGLOCK
   /// </summary>
   public static readonly NpgsqlTableHintLimited PagLock = new("PAGLOCK");

   /// <summary>
   /// READCOMMITTED
   /// </summary>
   public static readonly NpgsqlTableHintLimited ReadCommitted = new("READCOMMITTED");

   /// <summary>
   /// READCOMMITTEDLOCK
   /// </summary>
   public static readonly NpgsqlTableHintLimited ReadCommittedLock = new("READCOMMITTEDLOCK");

   /// <summary>
   /// READPAST
   /// </summary>
   public static readonly NpgsqlTableHintLimited ReadPast = new("READPAST");

   /// <summary>
   /// REPEATABLEREAD
   /// </summary>
   public static readonly NpgsqlTableHintLimited RepeatableRead = new("REPEATABLEREAD");

   /// <summary>
   /// ROWLOCK
   /// </summary>
   public static readonly NpgsqlTableHintLimited RowLock = new("ROWLOCK");

   /// <summary>
   /// SERIALIZABLE
   /// </summary>
   public static readonly NpgsqlTableHintLimited Serializable = new("SERIALIZABLE");

   /// <summary>
   /// SNAPSHOT
   /// </summary>
   public static readonly NpgsqlTableHintLimited Snapshot = new("SNAPSHOT");

   /// <summary>
   /// TABLOCK
   /// </summary>
   public static readonly NpgsqlTableHintLimited TabLock = new("TABLOCK");

   /// <summary>
   /// TABLOCKX
   /// </summary>
   public static readonly NpgsqlTableHintLimited TabLockx = new("TABLOCKX");

   /// <summary>
   /// UPDLOCK
   /// </summary>
   public static readonly NpgsqlTableHintLimited UpdLock = new("UPDLOCK");

   /// <summary>
   /// XLOCK
   /// </summary>
   public static readonly NpgsqlTableHintLimited XLock = new("XLOCK");

   private readonly string _value;

   private NpgsqlTableHintLimited(string value)
   {
      _value = value ?? throw new ArgumentNullException(nameof(value));
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      if (ReferenceEquals(null, obj))
         return false;
      if (ReferenceEquals(this, obj))
         return true;
      if (obj.GetType() != this.GetType())
         return false;
      return Equals((NpgsqlTableHintLimited)obj);
   }

   /// <inheritdoc />
   public bool Equals(NpgsqlTableHintLimited? other)
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
}
