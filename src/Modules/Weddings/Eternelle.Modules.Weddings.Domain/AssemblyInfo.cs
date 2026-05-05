using System.Runtime.CompilerServices;

// Allow the Infrastructure layer to access internal members — specifically
// FromPersistence() factory methods on value objects, which bypass Create()
// validation for values that were already validated on write.
[assembly: InternalsVisibleTo("Eternelle.Modules.Weddings.Infrastructure")]
