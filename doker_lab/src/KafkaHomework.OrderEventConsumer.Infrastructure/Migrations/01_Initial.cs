using System;
using FluentMigrator;

using KafkaHomework.OrderEventConsumer.Infrastructure.Common;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Migrations;

[Migration(1, "Initial migration")]
public sealed class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
        create table Orders (
                    item_id bigint not null unique ,
                    quantity_reserved int not null ,
                    quantity_sold int ,
                    quantity_canceled int ,
                    last_updated timestamp not null default now()
                );
        create table seller_payments (
                    seller_id bigint not null ,         
                    product_id bigint not null,       
                    currency varchar(3) not null ,      
                    total_sales decimal(20, 9) not null default 0,  
                    quantity_sold int not null default 0, 
                    primary key (seller_id, product_id, currency) 
                );

";
    protected override string GetDownSql(IServiceProvider services) => @"
        drop table if exists Orders;
        drop table if exists seller_payments;
    ";
}
