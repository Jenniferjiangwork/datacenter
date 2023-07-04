
DROP procedure IF EXISTS `sp_portfolio_cash_flow_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_cash_flow_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_cashflow WHERE year=last_year and month=last_month;
	INSERT INTO rpt_portfolio_cashflow(status,lender,paymentmethod,year,month,number,amount,createddate)
	SELECT * FROM (
		SELECT
			'Highest' as status,
			(select lender from ams_loanaccount where loanaccountid=d.loanaccountid) as lender,
			'DDR' as PaymentMethod,
			DATE_FORMAT(dateofpayment,'%Y') as year,
			DATE_FORMAT(dateofpayment,'%m') as month,
			COUNT(*) as number,
			max(amount) as amount,
			curdate() as Createddate
		FROM ams_ddrbanking as d 
		where YEAR(dateofpayment) = last_year and MONTH(dateofpayment) = last_month
		GROUP BY lender
		union all
		SELECT
			'Paymentdue' as status,
			(select lender from ams_loanaccount where loanaccountid=d.loanaccountid) as lender,
			'DDR' as PaymentMethod,
			DATE_FORMAT(dateofpayment,'%Y') as year,
			DATE_FORMAT(dateofpayment,'%m') as month,
			COUNT(*) as number,
			SUM(amount) as amount,
			curdate() as Createddate
		FROM ams_ddrbanking as d
		WHERE YEAR(dateofpayment) = last_year and MONTH(dateofpayment) = last_month
		GROUP BY lender
		union all
		SELECT
			'Received' as status,
			(select lender from ams_loanaccount where loanaccountid=d.loanaccountid) as lender,
			'DDR' as PaymentMethod,
			DATE_FORMAT(dateofpayment,'%Y') as year,
			DATE_FORMAT(dateofpayment,'%m') as month,
			COUNT(*) as number,
			SUM(amount) as amount,
			curdate() as Createddate
		FROM ams_ddrbanking as d
		WHERE paymentstatus='S' and 
		YEAR(dateofpayment) = last_year and MONTH(dateofpayment) = last_month
		GROUP BY lender
		
		/* payment method='Cash' */
		union all
		
		SELECT
			'Highest' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Cash' as PaymentMethod,
			DATE_FORMAT(transactiondate,'%Y') as year,
			DATE_FORMAT(transactiondate,'%m') as month,
			COUNT(*) as number,
			max(value*-1) as amount,
			curdate() as Createddate
		FROM ams_ledgeracc as l 
		WHERE PaymentStatus='cash' and 
		YEAR(transactiondate) = last_year and MONTH(transactiondate) = last_month
		GROUP BY lender
		union all
		SELECT
			'Paymentdue' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Cash' as PaymentMethod,
			DATE_FORMAT(currentpaymentdate,'%Y') as year,
			DATE_FORMAT(currentpaymentdate,'%m') as month,
			COUNT(*) as number,
			SUM(ifnull(currentpaymentamount,0)),
			curdate() as Createddate
		FROM ams_transaction as l 
		WHERE PaymentMethod<>'Direct Debit' and PaymentMethod<>'Card Over Phone' and deleteflag=0 and 
		YEAR(currentpaymentdate) = last_year and MONTH(currentpaymentdate) = last_month
		GROUP BY lender
		union all
		SELECT
			'Received' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Cash' as PaymentMethod,
			DATE_FORMAT(transactiondate,'%Y') as year,
			DATE_FORMAT(transactiondate,'%m') as month,
			COUNT(*) as number,
			SUM(value*-1) as amount,
			curdate() as Createddate
		FROM ams_ledgeracc as l 
		WHERE PaymentStatus='cash' and 
		YEAR(transactiondate) = last_year and MONTH(transactiondate) = last_month
		GROUP BY lender
		
		/* payment method='Card' */
		union all
		
		SELECT
			'Highest' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Card' as PaymentMethod,
			DATE_FORMAT(transactiondate,'%Y') as year,
			DATE_FORMAT(transactiondate,'%m') as month,
			COUNT(*) as number,
			max(value*-1) as amount,
			curdate() as Createddate
		FROM ams_ledgeracc as l 
		WHERE PaymentStatus='Card Over Phone' and 
		YEAR(transactiondate) = last_year and MONTH(transactiondate) = last_month
		GROUP BY lender
		union all
		SELECT
			'Paymentdue' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Card' as PaymentMethod,
			DATE_FORMAT(currentpaymentdate,'%Y') as year,
			DATE_FORMAT(currentpaymentdate,'%m') as month,
			COUNT(*) as number,
			SUM(ifnull(currentpaymentamount,0)),
			curdate() as Createddate
		FROM ams_transaction as l 
		WHERE PaymentMethod='Card Over Phone' and deleteflag=0 and 
		YEAR(currentpaymentdate) = last_year and MONTH(currentpaymentdate) = last_month
		GROUP BY lender
		union all
		SELECT
			'Received' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Card' as PaymentMethod,
			DATE_FORMAT(transactiondate,'%Y') as year,
			DATE_FORMAT(transactiondate,'%m') as month,
			COUNT(*) as number,
			SUM(value*-1) as amount,
			curdate() as Createddate
		FROM ams_ledgeracc as l
		WHERE PaymentStatus='Card Over Phone' and 
		YEAR(transactiondate) = last_year and MONTH(transactiondate) = last_month
		GROUP BY lender
	) as q1 ORDER BY year desc,month desc;

	/* Payment Success */
	
	DROP TABLE IF EXISTS tempReceive;
	DROP TABLE IF EXISTS tempPayment;

	CREATE TEMPORARY TABLE tempReceive
	(
		status varchar(45),
		lender varchar(45),
		PaymentMethod varchar(45),
		number int,
		amount decimal(18,2)
	);

	CREATE TEMPORARY TABLE tempPayment
	(
		status varchar(45),
		lender varchar(45),
		PaymentMethod varchar(45),
		number int,
		amount decimal(18,2)
	);

	INSERT INTO tempReceive 
		select * from (
			SELECT
			'Received' as status,
			(select lender from ams_loanaccount where loanaccountid=d.loanaccountid) as lender,
			'DDR' as PaymentMethod,
			COUNT(*) as number,
			SUM(amount) as amount
			FROM ams_ddrbanking as d
			WHERE paymentstatus='S' and 
			YEAR(dateofpayment) = last_year and MONTH(dateofpayment) = last_month
			GROUP BY lender

			union all

			select
			'Received' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Cash' as PaymentMethod,
			COUNT(*) as number,
			SUM(value*-1) as amount
			FROM ams_ledgeracc as l 
			WHERE PaymentStatus='cash' and 
			YEAR(transactiondate) = last_year and MONTH(transactiondate) = last_month
			GROUP BY lender

			union all

			SELECT
			'Received' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Card' as PaymentMethod,
			COUNT(*) as number,
			SUM(value*-1) as amount
			FROM ams_ledgeracc as l
			WHERE PaymentStatus='Card Over Phone' and 
			YEAR(transactiondate) = last_year and MONTH(transactiondate) = last_month
			GROUP BY lender) as q1;

	INSERT INTO tempPayment 
		select * from(
			SELECT
			'Paymentdue' as status,
			(select lender from ams_loanaccount where loanaccountid=d.loanaccountid) as lender,
			'DDR' as PaymentMethod,
			 COUNT(*) as number,
			 SUM(amount) as amount
			FROM ams_ddrbanking as d
			WHERE YEAR(dateofpayment) = last_year and MONTH(dateofpayment) = last_month
			GROUP BY lender

			union all

			SELECT
			'Paymentdue' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Cash' as PaymentMethod,
			COUNT(*) as number,
			SUM(ifnull(currentpaymentamount,0))
			FROM ams_transaction as l 
			WHERE PaymentMethod<>'Direct Debit' and PaymentMethod<>'Card Over Phone' and deleteflag=0 and 
			YEAR(currentpaymentdate) = last_year and MONTH(currentpaymentdate) = last_month
			GROUP BY lender

			union all

			SELECT
			'Paymentdue' as status,
			(select lender from ams_loanaccount where loanaccountid=l.loanaccountid) as lender,
			'Card' as PaymentMethod,
			COUNT(*) as number,
			SUM(ifnull(currentpaymentamount,0))
			FROM ams_transaction as l 
			WHERE PaymentMethod='Card Over Phone' and deleteflag=0 and 
			YEAR(currentpaymentdate) = last_year and MONTH(currentpaymentdate) = last_month
			GROUP BY lender) as q1;

	BEGIN
		DECLARE received_done int default false;
		DECLARE received_status varchar(45);
		DECLARE received_lender varchar(45);
		DECLARE received_PaymentMethod varchar(45);
		DECLARE received_number int;
		DECLARE received_amount decimal(18,2);
		DECLARE cur_received cursor for(select status,lender,PaymentMethod,number,amount from tempReceive);
		DECLARE continue handler for not found set received_done = true;
		open cur_received;
		cur_received_loop : LOOP
			fetch cur_received into received_status,received_lender, received_PaymentMethod, received_number, received_amount;
			
			if received_done then
				close cur_received;
				leave cur_received_loop;
			end if;
	
			block2: BEGIN 
				DECLARE payment_done int default false;
				DECLARE payment_status varchar(45);
				DECLARE payment_lender varchar(45);
				DECLARE payment_PaymentMethod varchar(45);
				DECLARE payment_number int ;
				DECLARE payment_amount decimal(18,2) ;
				DECLARE cur_payment cursor for (select status,lender,PaymentMethod,number,amount from tempPayment);
				DECLARE continue handler for not found set payment_done = true;
				open cur_payment;
				cur_payment_loop: LOOP
				fetch cur_payment into payment_status,payment_lender,payment_PaymentMethod,payment_number,payment_amount;
				if payment_done then 
					set payment_done = false;
					close cur_payment;
					leave cur_payment_loop;
				end if;

				insert into rpt_portfolio_cashflow(status,lender,paymentmethod,year,month,number,amount,createddate)
				select * from (
					select 'Payment Success' as status,
					lender,
					PaymentMethod,
					year,
					month,
					(received_number/payment_number)*100 as number ,
					(received_amount/payment_amount)*100 as amount,
					curdate() as createddate
					from rpt_portfolio_cashflow
					where (lender = payment_lender and lender = received_lender) and
					(PaymentMethod=received_PaymentMethod and PaymentMethod=payment_PaymentMethod)
					and year = last_year and month = last_month
					group by lender,PaymentMethod
				) as q1;

				end loop cur_payment_loop;
			end block2;
		end loop cur_received_loop;

	END;

END$$

DELIMITER ;

