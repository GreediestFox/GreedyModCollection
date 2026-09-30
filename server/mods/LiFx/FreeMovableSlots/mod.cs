// Frees ~186 client/server ShapeBaseImageData datablock slots (shared pool ~1024): dev-only 'Starting Island Decoration' (parent 1457) and quest (parent 1540) movable types that have no recipe and are placed nowhere become non-movable. Ids: 186.

if (!isObject(LiFxFreeMovableSlots))
{
    new ScriptObject(LiFxFreeMovableSlots)
    {
    };
}

package LiFxFreeMovableSlots
{
    function LiFxFreeMovableSlots::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxFreeMovableSlots);
    }
    function LiFxFreeMovableSlots::version() {
        return "1.0.0";
    }
    function LiFxFreeMovableSlots::dbChanges() {
        dbi.Update("UPDATE `objects_types` SET IsMovableObject=0 WHERE IsMovableObject=1 AND ID IN (1440,1441,1442,1544,1545,1546,1547,1548,1549,1550,1652,1653,1654,1655,1656,1673,1674,1675,1676,1677,1678,1679,1680,1681,1687,1688,1689,1690,1691,1692,1693,1694,1695,1696,1697,1698,1742,1743,1744,1745,1748,1749,1750,1751,1752,1753,1754,1755,1756,1757,1758,1759,1760,1761,1762,1763,1764,1765,1766,1767,1768,1769,1770,1771,1772,1773,1774,1775,1776,1777,1778,1779,1780,1781,1782,1783,1784,1785,1786,1787,1788,1789,1790,1791,1792,1793,1794,1795,1796,1797,1798,1799,1800,1801,1802,1803,1804,1805,1806,1807,1808,1809,1810,1811,1812,1813,1814,1815,1816,1817,1818,1819,1820,1821,1822,1823,1824,1825,1826,1827,1828,1829,1830,1831,1832,1833,1834,1835,1836,1837,1838,1839,1840,1841,1842,1843,1844,1845,1846,1847,1848,1849,1850,1851,1852,1853,1854,1855,1856,1857,1858,1859,1860,1861,1862,1863,1864,1865,1866,1867,1868,1869,1870,1871,1872,1873,1874,1875,1876,1877,1878,1879,1880,1881,1882,1883,1884,1885,1886,1887,1888,1889,1890,1891,1899,1900)");
    }
};
activatePackage(LiFxFreeMovableSlots);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxFreeMovableSlots);
