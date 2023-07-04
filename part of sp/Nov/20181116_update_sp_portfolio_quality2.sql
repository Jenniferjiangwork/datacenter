
DROP procedure IF EXISTS `sp_portfolio_quality2`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_quality2`(IN p_lender varchar(300))
BEGIN

	SELECT *,
		CONCAT(year,month) as yearmonth
	FROM rpt_portfolio_quality2
	WHERE (ifnull(p_lender,'') = '' or FIND_IN_SET(lender,p_lender) > 0)
	ORDER BY year desc,month desc;

END$$

DELIMITER ;

