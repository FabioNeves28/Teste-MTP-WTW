IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE TABLE [Clientes] (
        [Id] uniqueidentifier NOT NULL,
        [Nome] nvarchar(150) NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [Documento] varchar(14) NOT NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE TABLE [Produtos] (
        [Id] uniqueidentifier NOT NULL,
        [Nome] nvarchar(150) NOT NULL,
        [Descricao] nvarchar(1000) NOT NULL,
        [Preco] decimal(18,2) NOT NULL,
        [QuantidadeEstoque] int NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Produtos] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE TABLE [Pedidos] (
        [Id] uniqueidentifier NOT NULL,
        [ClienteId] uniqueidentifier NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [CriadoEm] datetime2 NOT NULL,
        [AtualizadoEm] datetime2 NULL,
        CONSTRAINT [PK_Pedidos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Pedidos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE TABLE [ItensPedido] (
        [Id] uniqueidentifier NOT NULL,
        [PedidoId] uniqueidentifier NOT NULL,
        [ProdutoId] uniqueidentifier NOT NULL,
        [NomeProduto] nvarchar(150) NOT NULL,
        [PrecoUnitario] decimal(18,2) NOT NULL,
        [Quantidade] int NOT NULL,
        CONSTRAINT [PK_ItensPedido] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ItensPedido_Pedidos_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedidos] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ItensPedido_Produtos_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_Documento] ON [Clientes] ([Documento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_Email] ON [Clientes] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ItensPedido_PedidoId_ProdutoId] ON [ItensPedido] ([PedidoId], [ProdutoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE INDEX [IX_ItensPedido_ProdutoId] ON [ItensPedido] ([ProdutoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE INDEX [IX_Pedidos_ClienteId] ON [Pedidos] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    CREATE INDEX [IX_Pedidos_Status] ON [Pedidos] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929144311_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260929144311_Inicial', N'10.0.12');
END;

COMMIT;
GO

