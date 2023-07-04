
DROP procedure IF EXISTS `sp_portfolio_loc_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_loc_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_loc WHERE status='Available LOC';
	DELETE FROM rpt_portfolio_loc WHERE year=last_year and month=last_month;

	INSERT INTO rpt_portfolio_loc(status,lender,year,month,number,amount,createddate)
	select * from (
		SELECT 'Available LOC'as status,lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate, '%m') as month,
			COUNT(*) as number,
			SUM(maxdrawdown) as amount,
			curdate() as createddate
		FROM ams_loanaccount
		WHERE status <> 'Recovery' AND maxdrawdown >= 1000
		GROUP BY lender
	
	union all
	
		SELECT 'Available LOC Per Month' as status ,lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate, '%m') as month,
			COUNT(*) as number,
			SUM(maxdrawdown) as amount,
			curdate() as createddate
		FROM ams_loanaccount
		WHERE status <> 'Recovery' AND maxdrawdown >= 1000 AND
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender
	
	union all
	
		SELECT
			'New LOC' as status,
			lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate,'%m') as month,
			COUNT(*) as number,
			SUM(drawdownamount) as amount,
			curdate() as createddate
		FROM ams_loanlinecredit
		WHERE YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender
	) as q1
	order by year desc,month desc;

END$$

DELIMITER ;

