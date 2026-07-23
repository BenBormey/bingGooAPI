using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Product;
using JuJuBiAPI.Models.ProductScale;
using JuJuBiAPI.Queries;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace JuJuBiAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _connection;

        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<CreateProductDto> CreateAsync(CreateProductDto product)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // Check duplicate barcode across ALL barcode columns
                // (PK is ProNumY, but pack/case/SKU duplicates cause lookup bugs later)
                var exists = await _connection.ExecuteScalarAsync<int>(
                    ProductQueries.ExistsByBarcodes,
                    new
                    {
                        product.ProNumY,
                        ProNumYP = product.ProNumYP ?? "",
                        ProNumYC = product.ProNumYC ?? ""
                    },
                    transaction);

                if (exists > 0)
                    throw new Exception("Product Number (Unit/Pack/Case) already exists.");

                var productId = await _connection.ExecuteScalarAsync<int>(
                    ProductQueries.Insert,
                    product,
                    transaction);

                // Insert Product Scale
                if (product.ProductScale != null)
                {
                    await _connection.ExecuteAsync(
                        ProductQueries.InsertScale,
                        new
                        {
                            ProId = productId,
                            product.ProductScale.CTNPerPallet,
                            product.ProductScale.UOMCode,
                            product.ProductScale.Width,
                            product.ProductScale.Length,
                            product.ProductScale.Height,
                            product.ProductScale.CBMPerCTN,
                            product.ProductScale.NetWeight,
                            product.ProductScale.GrossWeight,
                            CreatedDate = product.ProductScale.CreatedDate ?? DateTime.Now,
                            product.ProductScale.Status
                        },
                        transaction);
                }

                transaction.Commit();

                product.Id = productId;

                return product;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }

        public async Task<List<ProductListDto>> GetAllAsync()
        {
            var result = await _connection.QueryAsync<ProductListDto, ProductScaleDto, ProductListDto>(
                ProductQueries.GetAll,
                (product, scale) =>
                {
                    product.ProductScale = scale;
                    return product;
                },
                splitOn: "Id");

            return result.ToList();
        }

        public async Task<List<ProductListDto>> SearchByNameAsync(string name)
        {
            var result = await _connection.QueryAsync<ProductListDto, ProductScaleDto, ProductListDto>(
                ProductQueries.SearchByName,
                (product, scale) =>
                {
                    product.ProductScale = scale;
                    return product;
                },
                new { Name = $"%{name}%" },
                splitOn: "Id");

            return result.ToList();
        }

        public async Task<List<ProductListDto>> SearchBySkuAsync(string sku)
        {
            var result = await _connection.QueryAsync<ProductListDto, ProductScaleDto, ProductListDto>(
                ProductQueries.SearchBySku,
                (product, scale) =>
                {
                    product.ProductScale = scale;
                    return product;
                },
                new { Sku = $"%{sku}%" },
                splitOn: "Id");

            return result.ToList();
        }

        public async Task<ProductListDto?> GetByIdAsync(int id)
        {
            var result = await _connection.QueryAsync<ProductListDto, ProductScaleDto, ProductListDto>(
                ProductQueries.GetById,
                (product, scale) =>
                {
                    product.ProductScale = scale;
                    return product;
                },
                new { Id = id },
                splitOn: "Id");

            return result.FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(UpdateProductDto product)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // Check duplicate Product Number
                var exists = await _connection.ExecuteScalarAsync<int>(
                    ProductQueries.ExistsByNameOtherId,
                    new { product.ProNumY, product.ProID },
                    transaction);

                if (exists > 0)
                    throw new Exception("Product Number already exists.");

                // Update Product
                var rows = await _connection.ExecuteAsync(
                    ProductQueries.Update,
                    product,
                    transaction);

                // Update Product Scale
                if (product.ProductScale != null)
                {
                    var checkScale = await _connection.ExecuteScalarAsync<int>(
                        ProductQueries.CheckScale,
                        new { ProId = product.ProID },
                        transaction);

                    if (checkScale > 0)
                    {
                        await _connection.ExecuteAsync(
                            ProductQueries.UpdateScale,
                            new
                            {
                                ProId = product.ProID,
                                product.ProductScale.CTNPerPallet,
                                product.ProductScale.UOMCode,
                                product.ProductScale.Width,
                                product.ProductScale.Length,
                                product.ProductScale.Height,
                                product.ProductScale.CBMPerCTN,
                                product.ProductScale.NetWeight,
                                product.ProductScale.GrossWeight,
                                CreatedDate = product.ProductScale.CreatedDate ?? DateTime.Now,
                                product.ProductScale.Status
                            },
                            transaction);
                    }
                    else
                    {
                        await _connection.ExecuteAsync(
                            ProductQueries.InsertScaleUpdate,
                            new
                            {
                                ProId = product.ProID,
                                product.ProductScale.CTNPerPallet,
                                product.ProductScale.UOMCode,
                                product.ProductScale.Width,
                                product.ProductScale.Length,
                                product.ProductScale.Height,
                                product.ProductScale.CBMPerCTN,
                                product.ProductScale.NetWeight,
                                product.ProductScale.GrossWeight,
                                CreatedDate = product.ProductScale.CreatedDate ?? DateTime.Now,
                                product.ProductScale.Status
                            },
                            transaction);
                    }
                }

                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(ProductQueries.Delete, new { Id = id });
            return rows > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var count = await _connection.ExecuteScalarAsync<int>(ProductQueries.Exists, new { Id = id });
            return count > 0;
        }
        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            var product = (await _connection.QueryAsync<Product, ProductScaleDto, Product>(
                ProductQueries.GetByBarcodeProc,
                (p, scale) =>
                {
                    p.ProductScale = scale;
                    return p;
                },
                new
                {
                    Barcode = barcode
                },
                commandType: CommandType.StoredProcedure,
                splitOn: "Id"
            )).FirstOrDefault();

            if (product != null)
            {
                product.Status = await _connection.ExecuteScalarAsync<string?>(
                    ProductQueries.GetStatusById,
                    new { Id = product.ProID });
            }

            return product;
        }

        public async Task<bool> UpdateCaseNumberAsync(int id, string? caseNumber)
        {
            var rows = await _connection.ExecuteAsync(ProductQueries.UpdateCaseNumber, new { Id = id, CaseNumber = caseNumber });

            return rows > 0;
        }

        public async Task<bool> UpdateBarcodeAsync(int id, string? barcode)
        {
            var rows = await _connection.ExecuteAsync(ProductQueries.UpdateBarcode, new { Id = id, Barcode = barcode });

            return rows > 0;
        }

        public async Task<bool> UpdateOldBarcodeAsync(int id, string? oldBarcode)
        {
            var rows = await _connection.ExecuteAsync(ProductQueries.UpdateOldBarcode, new { Id = id, OldBarcode = oldBarcode });

            return rows > 0;
        }

        public async Task<bool> UpdatePackNumberAsync(int id, string? packNumber)
        {
            var rows = await _connection.ExecuteAsync(ProductQueries.UpdatePackNumber, new { Id = id, PackNumber = packNumber });

            return rows > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string? status)
        {
            var rows = await _connection.ExecuteAsync(ProductQueries.UpdateStatus, new { Id = id, Status = status });

            return rows > 0;
        }
    }
}
