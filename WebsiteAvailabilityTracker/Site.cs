﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public class Site : IComparable<Site>, IComparer<Site>
    {
        public string Address { get; set; }
        public uint CheckingFrequency { get; set; }

        private int checkingTimeStep = SiteChecker.GetCheckingTimeStep();
        private int maxCheckingInterval = SiteChecker.GetMaxCheckingInterval();

        public Site(string siteAddress, uint frequency)
        {
            Address = siteAddress;
            //частота проверки изменяется с шагом в полсекунды и не может быть меньше 10 секунд
            if ((frequency % checkingTimeStep == 0) && (frequency <= maxCheckingInterval))
            {
                CheckingFrequency = frequency;
            }
            else
            {
                throw new SiteCheckingFrequencyException("Incorrect frequency value");
            }
        }

        public int Compare(Site s1, Site s2)
        {
            if (s1 == null && s2 == null)
            {
                return 0;
            }
            if (s1 != null && s2 == null)
            {
                return 1;
            }
            if (s1 == null && s2 != null)
            {
                return -1;
            }
            //на этом этапе оба объекта гарантированно != null
            int addressCompareResult = String.Compare(s1.Address, s2.Address);
            if (addressCompareResult != 0)
            {
                return addressCompareResult;
            }
            //если адреса одинаковы, меньше тот, который проверяется чаще
            //альтернативный вариант: вызывать исключение. на данный момент нецелесообразен
            if (s1.CheckingFrequency < s2.CheckingFrequency)
            {
                return -1;
            }
            if (s1.CheckingFrequency > s2.CheckingFrequency)
            {
                return 1;
            }
            return 0;
        }
        public int CompareTo(Site s)
        {
            if (s == null)
            {
                return -1;
            }
            else
            {
                string site1Address = this.Address;
                int addressCompareResult = site1Address.CompareTo(s.Address);
                if (addressCompareResult != 0)
                {
                    return addressCompareResult;
                }
                //если адреса одинаковы, меньше тот, который проверяется чаще
                //альтернативный вариант: вызывать исключение. на данный момент нецелесообразен
                if (this.CheckingFrequency < s.CheckingFrequency)
                {
                    return -1;
                }
                if (this.CheckingFrequency > s.CheckingFrequency)
                {
                    return 1;
                }
                return 0;
            }
        }

        public override bool Equals(Object obj)
        {
            //так как уникальность сайтов отслеживается по адресам, сравнение по частоте лишено смысла
            if (obj == null)
            {
                return false;
            }
            Site s = obj as Site;
            if (s as Site == null)
            {
                return false;
            }
            return this.Address == s.Address;
        }

        public bool Equals(Site s)
        {
            //так как уникальность сайтов отслеживается по адресам, сравнение по частоте лишено смысла
            if (s == null)
            {
                return false;
            }
            return this.Address == s.Address;
        }

        public override int GetHashCode()
        {

            int GetNumberFromText(string processingString)
            {
                int number = 0;
                foreach (char symbol in processingString)
                {
                    number += (int)symbol;
                }
                return number;
            }

            int hash = 17;

            hash *= GetNumberFromText(this.Address);
            //так как uint имеет в 2 раза больший положительный диапазон, мы делим uint поле на 2.
            //при этом шанс совпадения хешей меньше и виден более явно, чем при приведении без деления
            hash *= (int) (this.CheckingFrequency / 2);
            //при добавлении полей - дописать сюда их хеширование

            return hash;
        }
    }

    class SiteCheckingFrequencyException : ArgumentException
    {
        public SiteCheckingFrequencyException(string message)
            : base(message)
        { }
    }
}