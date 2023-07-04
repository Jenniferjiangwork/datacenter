
DROP procedure IF EXISTS `sp_portfolio_quality1_security`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_quality1_security`(IN p_lender varchar(300))
BEGIN

	SELECT *,
		CONCAT(year,month) as yearmonth
	FROM rpt_portfolio_quality1_security
	WHERE (ifnull(p_lender,'') = '' or FIND_IN_SET(lender,p_lender) > 0)
	ORDER BY year desc,month desc;

END$$

DELIMITER ;

