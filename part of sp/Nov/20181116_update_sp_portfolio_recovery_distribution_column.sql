
DROP procedure IF EXISTS `sp_portfolio_recovery_distribution_column`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_recovery_distribution_column`(IN p_lender varchar(300))
BEGIN

	SELECT distinct
		CONCAT(year,month) as yearmonth
	FROM rpt_portfolio_recovery_distribution
	WHERE (ifnull(p_lender,'') = '' or FIND_IN_SET(lender,p_lender) > 0)
	ORDER BY year desc,month desc;

END$$

DELIMITER ;

