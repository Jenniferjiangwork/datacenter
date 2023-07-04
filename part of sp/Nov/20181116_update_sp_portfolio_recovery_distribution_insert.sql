
DROP procedure IF EXISTS `sp_portfolio_recovery_distribution_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_recovery_distribution_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);
	
	CREATE TEMPORARY TABLE tempRecDistribution
	(
		lender varchar(300) not null,
		numb int
	);
	
	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));
	
	DELETE FROM rpt_portfolio_recovery_distribution WHERE year=last_year and month=last_month;
	
	INSERT INTO tempRecDistribution
	SELECT q1.lender,sum(q1.cnt) FROM(
		SELECT 'Collection' as status,lender,count(*) as cnt
		FROM ams_loanaccount
		WHERE agentjob in('Score Collection','JBS Collection','Swift Collection','Express Mercantile','Access Mercantile')
		and YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender
		
		UNION ALL
		SELECT 'Field' as status ,lender,count(*) as cnt FROM ams_loanaccount WHERE recoverystatus in ('Field - Opened','Field - Closed','Field - Placement')
		and YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender
		
		UNION ALL
		SELECT 'Legal' as status ,lender,count(*) as cnt FROM ams_loanaccount WHERE recoverystatus in('Legal - Placement','Legal - Opened','Legal - Closed')
		and YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender
		
		UNION ALL
		SELECT 'Company Prepared' as status ,lender,count(*)as cnt from ams_loanaccount WHERE recoverystatus in('Company - Prepare Stat Demand')
		and YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender
		
	)as q1 GROUP BY lender;
	select * from tempRecDistribution;
	BEGIN
		DECLARE tempnumb int;
		DECLARE templender varchar(45);
		DECLARE done int DEFAULT FALSE;
		DECLARE cur_recovery CURSOR for (select * from tempRecDistribution);
		DECLARE CONTINUE HANDLER FOR NOT FOUND SET done=TRUE;
		OPEN cur_recovery;
		READ_LOOP: LOOP
			FETCH cur_recovery INTO templender,tempnumb;
			IF done THEN
				LEAVE READ_LOOP;
			END IF;
		
			INSERT INTO rpt_portfolio_recovery_distribution(status,lender,year,month,number,percent,createddate)
			SELECT * FROM(
				SELECT 'Collections' as status,lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate, '%m') as month,
					count(*) as number,
					(count(*)/tempnumb * 100) as percent,
					curdate() as createddate
				FROM ams_loanaccount
				WHERE agentjob in('Score Collection','JBS Collection','Swift Collection') and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and lender=templender
				GROUP BY lender

				UNION ALL

				SELECT 'Merchantile' as status,lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate, '%m') as month,
					count(*) as number,
					(count(*)/tempnumb * 100) as percent,
					curdate() as createddate 
				FROM ams_loanaccount
				WHERE (agentjob in('Express Mercantile','Access Mercantile') OR recoverystatus in ('Field - Opened','Field - Closed','Field - Placement')) and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and lender=templender
				GROUP BY lender

				UNION ALL

				SELECT 'Legal' as status,lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate, '%m') as month,
					count(*) as number,
					(count(*)/tempnumb * 100) as percent,
					curdate() as createddate 
				FROM ams_loanaccount
				WHERE recoverystatus in('Legal - Placement','Legal - Opened','Legal - Closed') and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and lender=templender
				GROUP BY lender

				UNION ALL

				SELECT 'Company - Prepare Stat' as status,lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate, '%m') as month,
					count(*) as number,
					(count(*)/tempnumb * 100) as percent,
					curdate() as createddate 
				FROM ams_loanaccount
				WHERE recoverystatus in('Company - Prepare Stat Demand') and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and lender=templender
				GROUP BY lender
			) as q2 
			ORDER BY year desc,month desc;
		END LOOP;
		CLOSE cur_recovery;
	END;
	
	DROP TABLE IF EXISTS tempRecDistribution;

END$$

DELIMITER ;

